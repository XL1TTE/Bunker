using Bunker.GameService.Messages;
using Bunker.GameService.Persistence.Contracts.Queries;
using Wolverine;
using Wolverine.Attributes;

namespace Bunker.GameService.Features.CastVote;

[WolverineHandler]
public static class CastVoteHandler
{
    public static async Task<CastVote.Result> Handle(
        CastVote command,
        IGameQueries queries,
        IMessageContext messaging)
    {
        var game = await queries.GetGameAsync(command.GameId);
        if (game is null)
            return CastVote.Failure("Game not found.");

        var voter = game.Participants.FirstOrDefault(p => p.AccountId == command.AccountId);
        if (voter is null)
            return CastVote.Failure("You are not in this game.");

        if (game.Phase != "Voting")
            return CastVote.Failure("Voting is only allowed in the Voting phase.");

        if (game.CurrentTurnIndex < 0
            || game.CurrentTurnIndex >= game.TurnOrder.Count
            || game.TurnOrder[game.CurrentTurnIndex] != voter.Id)
        {
            return CastVote.Failure("It is not your turn.");
        }

        if (command.TargetParticipantId == voter.Id)
            return CastVote.Failure("You cannot vote for yourself.");

        var target = game.Participants.FirstOrDefault(p => p.Id == command.TargetParticipantId);
        if (target is null)
            return CastVote.Failure("Unknown vote target.");
        if (target.Eliminated)
            return CastVote.Failure("You cannot vote for an eliminated player.");

        if (game.VoteRound == 2 && !game.TiedParticipantIds.Contains(target.Id))
            return CastVote.Failure("Vote target must be one of the tied players.");

        await messaging.PublishAsync(new VoteRequested(
            GameId: command.GameId,
            VoterId: voter.Id,
            TargetId: target.Id));

        return CastVote.Success();
    }
}