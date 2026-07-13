using Bunker.GameService.Configuration;
using Bunker.GameService.GameConstruction;
using Bunker.GameService.Hubs;
using Bunker.GameService.Messages;
using Bunker.GameService.Persistence.Values;
using Bunker.GameService.Transfers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Wolverine;

namespace Bunker.GameService.Sagas;

public class GameSaga : Saga
{
    public Guid Id { get; set; }
    public Guid StartRequestId { get; set; }
    public Guid LobbyId { get; set; }
    public string HostId { get; set; } = "";
    public string Status { get; set; } = "Started";
    public DateTime CreatedAt { get; set; }
    public int BunkerCapacity { get; set; }
    public string Phase { get; set; } = "";
    public int RoundNumber { get; set; }
    public int CurrentTurnIndex { get; set; }
    public int TurnEpoch { get; set; }

    // Shuffled participant ids, built once at the first turn-based phase and reused for
    // every subsequent phase (eliminated participants are skipped during advancement).
    public List<string> TurnOrder { get; set; } = [];

    // Voting state (Phase 2c). Votes are cleared at the start of each voting round and after
    // each elimination; VoteRound is 1 for the first vote and 2 for the restricted revote.
    public int VoteRound { get; set; }
    public List<VoteData> Votes { get; set; } = [];
    public List<string> TiedParticipantIds { get; set; } = [];

    public BunkerCardData BunkerCard { get; set; } = new();
    public List<ParticipantData> Participants { get; set; } = [];

    public static (GameSaga, GameStartSucceeded, BeginBunkerIntroduction) Start(StartGame msg)
    {
        var gameId = Guid.NewGuid();

        var (participants, bunkerCard, bunkerCapacity) = GameInitializer.Initialize(
            msg.Participants,
            msg.ProfessionCards,
            msg.HobbiesCards,
            msg.AgeCards,
            msg.SexCards,
            msg.FactCards,
            msg.HealthCards,
            msg.LuggageCards,
            msg.BunkerCards);

        var saga = new GameSaga
        {
            Id = gameId,
            StartRequestId = msg.StartRequestId,
            LobbyId = msg.LobbyId,
            HostId = msg.HostId,
            Status = "Started",
            CreatedAt = DateTime.UtcNow,
            BunkerCapacity = bunkerCapacity,
            Phase = "BunkerIntroduction",
            RoundNumber = 1,
            CurrentTurnIndex = 0,
            TurnEpoch = 0,
            TurnOrder = [],
            BunkerCard = bunkerCard,
            Participants = participants
        };

        return (
            saga,
            new GameStartSucceeded(msg.StartRequestId, msg.LobbyId, gameId, $"/game/{gameId}"),
            new BeginBunkerIntroduction(GameId: gameId));
    }

    // --- Phase begin ---

    public async Task<object?> Handle(
        BeginBunkerIntroduction _,
        IHubContext<GameHub, IGameHub> hub,
        IOptions<GameTimingOptions> timings)
    {
        Phase = "BunkerIntroduction";
        var group = Id.ToString();

        await hub.Clients.Group(group).PhaseChanged(Phase, RoundNumber, (int)timings.Value.BunkerIntroduction.TotalSeconds);
        await hub.Clients.Group(group).BunkerCardRevealed(ToBunkerCardDto());

        return new BeginIntroDiscussion(GameId: Id).DelayedFor(timings.Value.BunkerIntroduction);
    }

    public async Task<object?> Handle(BeginIntroDiscussion _, IHubContext<GameHub, IGameHub> hub)
    {
        if (TurnOrder.Count == 0)
            TurnOrder = Shuffle(Participants.Select(p => p.Id).ToList());

        Phase = "IntroDiscussion";
        CurrentTurnIndex = -1;

        await hub.Clients.Group(Id.ToString()).PhaseChanged(Phase, RoundNumber, 0);

        return new AdvanceTurn(GameId: Id);
    }

    public async Task<object?> Handle(BeginReveal _, IHubContext<GameHub, IGameHub> hub)
    {
        Phase = "Reveal";
        CurrentTurnIndex = -1;

        await hub.Clients.Group(Id.ToString()).PhaseChanged(Phase, RoundNumber, 0);

        return new AdvanceTurn(GameId: Id);
    }

    // --- Turn advancement ---

    public async Task<object?> Handle(
        AdvanceTurn _,
        IHubContext<GameHub, IGameHub> hub,
        IOptions<GameTimingOptions> timings)
    {
        // Advance to the next non-eliminated participant in the fixed turn order.
        var nextIndex = -1;
        for (var i = CurrentTurnIndex + 1; i < TurnOrder.Count; i++)
        {
            var candidate = FindParticipant(TurnOrder[i]);
            if (candidate is not null && !candidate.Eliminated)
            {
                nextIndex = i;
                break;
            }
        }

        if (nextIndex < 0)
        {
            // Phase complete: flow into the next phase of the round loop.
            return Phase switch
            {
                "IntroDiscussion" => new BeginReveal(GameId: Id),
                "Reveal" => new BeginDiscussion(GameId: Id),
                "DiscussionClosing" => new BeginVoting(GameId: Id),
                "Voting" => new BeginElimination(GameId: Id),
                _ => null
            };
        }

        CurrentTurnIndex = nextIndex;
        TurnEpoch++;
        var participant = FindParticipant(TurnOrder[nextIndex])!;
        var group = Id.ToString();

        // Bots act instantly: perform the mock-random action and advance immediately.
        if (participant.Type == "Bot")
        {
            await ApplyBotTurnActionAsync(participant, hub, group);
            return new AdvanceTurn(GameId: Id);
        }

        await hub.Clients.Group(group).TurnChanged(participant.Id, Phase, CurrentTurnIndex, (int)timings.Value.TurnTimeout.TotalSeconds);

        return new TurnTimeout(GameId: Id, Phase, RoundNumber, CurrentTurnIndex, TurnEpoch)
            .DelayedFor(timings.Value.TurnTimeout);
    }

    public async Task<object?> Handle(TurnTimeout timeout, IHubContext<GameHub, IGameHub> hub)
    {
        // Idempotent guard (ADR 0004): a player who already acted or a phase that already
        // advanced makes the stale timeout a no-op.
        if (Phase != timeout.Phase || CurrentTurnIndex != timeout.TurnIndex || TurnEpoch != timeout.Epoch)
            return null;

        if (CurrentTurnIndex < 0 || CurrentTurnIndex >= TurnOrder.Count)
            return new AdvanceTurn(GameId: Id);

        var participant = FindParticipant(TurnOrder[CurrentTurnIndex]);
        if (participant is null)
            return new AdvanceTurn(GameId: Id);

        if (Phase == "Reveal")
        {
            var slot = RandomUnrevealed(participant);
            if (slot is not null)
            {
                slot.Revealed = true;
                await hub.Clients.Group(Id.ToString())
                    .AttributeRevealed(participant.Id, slot.Kind, slot.Value);
            }
            // No unrevealed attributes left -> the turn is auto-skipped.
        }

        // IntroDiscussion has no auto-action besides advancing.
        return new AdvanceTurn(GameId: Id);
    }

    // --- Player actions ---

    public async Task<object?> Handle(AttributeRevealRequested msg, IHubContext<GameHub, IGameHub> hub)
    {
        // Authoritative re-validation; the command handler already fast-failed bad requests.
        if (Phase != "Reveal") return null;
        if (CurrentTurnIndex < 0 || CurrentTurnIndex >= TurnOrder.Count) return null;
        if (TurnOrder[CurrentTurnIndex] != msg.ParticipantId) return null;

        var participant = FindParticipant(msg.ParticipantId);
        if (participant is null) return null;

        var slot = participant.Attributes.FirstOrDefault(a => a.Kind == msg.AttributeKind && !a.Revealed);
        if (slot is null) return null;

        slot.Revealed = true;
        await hub.Clients.Group(Id.ToString())
            .AttributeRevealed(participant.Id, slot.Kind, slot.Value);

        return new AdvanceTurn(GameId: Id);
    }

    // --- Phase 2c: Discussion / Voting / Elimination / Roulette / Finish ---

    public async Task<object?> Handle(
        BeginDiscussion _,
        IHubContext<GameHub, IGameHub> hub,
        IOptions<GameTimingOptions> timings)
    {
        Phase = "Discussion";
        CurrentTurnIndex = -1;

        await hub.Clients.Group(Id.ToString()).PhaseChanged(Phase, RoundNumber, (int)timings.Value.DiscussionFreeForAll.TotalSeconds);

        // 2-minute free-for-all chat, then turn-based closing turns.
        return new BeginClosingTurns(GameId: Id).DelayedFor(timings.Value.DiscussionFreeForAll);
    }

    public async Task<object?> Handle(BeginClosingTurns _, IHubContext<GameHub, IGameHub> hub)
    {
        Phase = "DiscussionClosing";
        CurrentTurnIndex = -1;

        await hub.Clients.Group(Id.ToString()).PhaseChanged(Phase, RoundNumber, 0);

        return new AdvanceTurn(GameId: Id);
    }

    public async Task<object?> Handle(BeginVoting _, IHubContext<GameHub, IGameHub> hub)
    {
        Phase = "Voting";
        CurrentTurnIndex = -1;
        Votes.Clear();
        VoteRound = VoteRound == 0 ? 1 : VoteRound; // first entry => 1; revote stays 2.

        await hub.Clients.Group(Id.ToString()).PhaseChanged(Phase, RoundNumber, 0);

        return new AdvanceTurn(GameId: Id);
    }

    public async Task<object?> Handle(VoteRequested msg, IHubContext<GameHub, IGameHub> hub)
    {
        // Authoritative re-validation; the command handler already fast-failed bad requests.
        if (Phase != "Voting") return null;
        if (CurrentTurnIndex < 0 || CurrentTurnIndex >= TurnOrder.Count) return null;
        if (TurnOrder[CurrentTurnIndex] != msg.VoterId) return null;

        var target = FindParticipant(msg.TargetId);
        if (target is null || target.Eliminated || target.Id == msg.VoterId) return null;
        if (VoteRound == 2 && !TiedParticipantIds.Contains(target.Id)) return null;

        RecordVote(msg.VoterId, target.Id);

        // Secret ballot: only signal that this player voted, never the target.
        await hub.Clients.Group(Id.ToString()).VoteCast(msg.VoterId);

        return new AdvanceTurn(GameId: Id);
    }

    public async Task<object?> Handle(BeginElimination _, IHubContext<GameHub, IGameHub> hub)
    {
        var tally = TallyVotes();
        var leaders = tally.Leaders;
        var group = Id.ToString();

        if (leaders.Count == 1)
        {
            var eliminatedId = leaders[0];
            return await EliminateAsync(eliminatedId, tally.ToDto(), hub, group);
        }

        // Tie (or everyone abstained -> treat all non-eliminated as tied).
        TiedParticipantIds = leaders.Count > 0
            ? leaders
            : Participants.Where(p => !p.Eliminated).Select(p => p.Id).ToList();

        if (VoteRound <= 1)
        {
            VoteRound = 2;
            Votes.Clear();
            return new BeginVoting(GameId: Id);
        }

        // Second vote still tied -> roulette.
        Votes.Clear();
        return new BeginRoulette(GameId: Id);
    }

    public async Task<object?> Handle(
        BeginRoulette _,
        IHubContext<GameHub, IGameHub> hub,
        IOptions<GameTimingOptions> timings)
    {
        Phase = "Roulette";

        await hub.Clients.Group(Id.ToString()).RouletteStarted(TiedParticipantIds);

        return new ResolveRoulette(GameId: Id).DelayedFor(timings.Value.RouletteDelay);
    }

    public async Task<object?> Handle(ResolveRoulette _, IHubContext<GameHub, IGameHub> hub)
    {
        if (TiedParticipantIds.Count == 0)
            return null;

        var eliminatedId = TiedParticipantIds[Random.Shared.Next(TiedParticipantIds.Count)];
        var tally = TallyVotes(); // likely empty after a roulette path; send what's known.
        var group = Id.ToString();

        await hub.Clients.Group(group).RouletteResult(eliminatedId);
        return await EliminateAsync(eliminatedId, tally.ToDto(), hub, group);
    }

    public async Task<object?> Handle(LeaveRequested msg, IHubContext<GameHub, IGameHub> hub, IMessageContext messaging)
    {
        var participant = FindParticipant(msg.ParticipantId);
        if (participant is null || participant.Eliminated)
            return null;

        participant.Eliminated = true;
        var group = Id.ToString();
        await hub.Clients.Group(group).Eliminated(participant.Id, new TallyDto(Array.Empty<VoteTallyEntry>(), 0));

        Votes.Clear();
        TiedParticipantIds.Clear();
        VoteRound = 0;

        if (participant.AccountId is string accountId)
            await messaging.PublishAsync(new PlayerLeftGame(LobbyId, Id, accountId));

        var remaining = Participants.Count(p => !p.Eliminated);
        if (remaining <= BunkerCapacity)
            return new BeginFinish(GameId: Id);

        var isTurnPhase = Phase is "Reveal" or "IntroDiscussion" or "DiscussionClosing";
        if (isTurnPhase
            && CurrentTurnIndex >= 0 && CurrentTurnIndex < TurnOrder.Count
            && TurnOrder[CurrentTurnIndex] == participant.Id)
        {
            return new AdvanceTurn(GameId: Id);
        }

        return null;
    }

    public async Task<object?> Handle(BeginNextRound _, IHubContext<GameHub, IGameHub> hub)
    {
        RoundNumber++;
        Votes.Clear();
        TiedParticipantIds.Clear();
        VoteRound = 0;

        await hub.Clients.Group(Id.ToString()).PhaseChanged("Reveal", RoundNumber, 0);

        return new BeginReveal(GameId: Id);
    }

    public async Task<object?> Handle(BeginFinish _, IHubContext<GameHub, IGameHub> hub)
    {
        Phase = "Finished";
        Status = "Finished";

        var survivors = Participants.Where(p => !p.Eliminated).Select(p => p.Id).ToList();
        await hub.Clients.Group(Id.ToString()).GameFinished(survivors);

        return new GameFinished(LobbyId, Id);
    }

    // --- Helpers ---

    private async Task ApplyBotTurnActionAsync(
        ParticipantData participant,
        IHubContext<GameHub, IGameHub> hub,
        string group)
    {
        if (Phase == "Reveal")
        {
            var slot = RandomUnrevealed(participant);
            if (slot is null)
                return; // Nothing left to reveal; the turn is auto-skipped.

            slot.Revealed = true;
            await hub.Clients.Group(group).AttributeRevealed(participant.Id, slot.Kind, slot.Value);
            return;
        }

        if (Phase == "Voting")
        {
            var target = RandomVoteTarget(participant);
            if (target is null)
                return; // No valid target (e.g. only the bot remains); abstain.

            RecordVote(participant.Id, target.Id);
            await hub.Clients.Group(group).VoteCast(participant.Id);
            return;
        }

        // IntroDiscussion / DiscussionClosing: nothing to do but advance.
    }

    private ParticipantData? FindParticipant(string id) => Participants.FirstOrDefault(p => p.Id == id);

    private void RecordVote(string voterId, string targetId)
    {
        var existing = Votes.FirstOrDefault(v => v.VoterId == voterId);
        if (existing is null)
            Votes.Add(new VoteData { VoterId = voterId, TargetId = targetId });
        else
            existing.TargetId = targetId;
    }

    private ParticipantData? RandomVoteTarget(ParticipantData voter)
    {
        var pool = Participants
            .Where(p => !p.Eliminated && p.Id != voter.Id)
            .Where(p => VoteRound != 2 || TiedParticipantIds.Contains(p.Id))
            .ToList();
        return pool.Count == 0 ? null : pool[Random.Shared.Next(pool.Count)];
    }

    private VoteTally TallyVotes()
    {
        var counts = new Dictionary<string, int>();
        var abstains = 0;
        foreach (var v in Votes)
        {
            if (v.TargetId is null)
            {
                abstains++;
                continue;
            }
            counts.TryGetValue(v.TargetId, out var c);
            counts[v.TargetId] = c + 1;
        }

        var max = counts.Count > 0 ? counts.Values.Max() : 0;
        var entries = counts
            .OrderByDescending(kv => kv.Value)
            .Select(kv => new VoteTallyEntry(kv.Key, kv.Value))
            .ToList();
        var leaders = max > 0
            ? counts.Where(kv => kv.Value == max).Select(kv => kv.Key).ToList()
            : [];

        return new VoteTally { Leaders = leaders, Abstains = abstains, Entries = entries };
    }

    private async Task<object?> EliminateAsync(
        string eliminatedId,
        TallyDto tally,
        IHubContext<GameHub, IGameHub> hub,
        string group)
    {
        var participant = FindParticipant(eliminatedId);
        if (participant is not null)
            participant.Eliminated = true;

        await hub.Clients.Group(group).Eliminated(eliminatedId, tally);

        Votes.Clear();
        TiedParticipantIds.Clear();
        VoteRound = 0;

        var remaining = Participants.Count(p => !p.Eliminated);
        return remaining <= BunkerCapacity
            ? new BeginFinish(GameId: Id)
            : new BeginNextRound(GameId: Id);
    }

    private sealed class VoteTally
    {
        public List<string> Leaders { get; init; } = [];
        public int Abstains { get; init; }
        public List<VoteTallyEntry> Entries { get; init; } = [];
        public TallyDto ToDto() => new(Entries, Abstains);
    }

    private static AttributeSlotData? RandomUnrevealed(ParticipantData participant)
    {
        var pool = participant.Attributes.Where(a => !a.Revealed).ToList();
        return pool.Count == 0 ? null : pool[Random.Shared.Next(pool.Count)];
    }

    private static List<string> Shuffle(List<string> source)
    {
        for (var i = source.Count - 1; i > 0; i--)
        {
            var j = Random.Shared.Next(i + 1);
            (source[i], source[j]) = (source[j], source[i]);
        }
        return source;
    }

    private BunkerCardDto ToBunkerCardDto() => new(
        BunkerCard.Id,
        BunkerCard.Catastrophe,
        BunkerCard.SurvivalDuration,
        BunkerCard.BunkerEnvironment);
}