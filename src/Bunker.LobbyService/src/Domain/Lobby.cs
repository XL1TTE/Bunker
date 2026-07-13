using System.Collections.ObjectModel;

namespace Bunker.LobbyService.Domain;

public enum LobbyState
{
    WaitingForPlayers,
    Starting,
    InGame
}

public record Lobby
{
    public readonly record struct Id(Guid Value)
    {
        public static Id Empty { get; } = new(Guid.Empty);
        public static Id New() => new(Guid.NewGuid());
        public static Id Restore(Guid value) => new(value);

        public override string ToString() => Value.ToString();
    }

    internal Lobby() { } // EF Core

    public Id PublicId { get; internal set; }
    public InviteCode InviteCode { get; internal set; }
    public LobbyName Name        { get; internal set; }
    public int Capacity          { get; internal set; }

    public PrivacyPolicy PrivacyPolicy { get; internal set; } = PrivacyPolicy.PublicPolicy();

    public LobbyState State { get; internal set; } = LobbyState.WaitingForPlayers;

    internal Collection<LobbyParticipant> Participants { get; set; } = [];
    internal Collection<LobbyCardPack> Packs { get; set; } = [];

    public IReadOnlyCollection<Player> Players => Participants.OfType<Player>().ToList().AsReadOnly();
    public IReadOnlyCollection<BotParticipant> Bots => Participants.OfType<BotParticipant>().ToList().AsReadOnly();

    public Player Host => Participants.OfType<Player>().First(x => x.Role == Role.Host);
}

public readonly record struct LobbyConfiguration(
    LobbyName Name,
    int Capacity,
    PrivacyPolicy Visibility,
    IReadOnlyCollection<Player> Players,
    IReadOnlyCollection<BotParticipant> Bots,
    IReadOnlyCollection<LobbyCardPack> CardPacks);

public static class LobbyFactory
{       
    extension (Lobby)
    {
        public static Lobby Create(int capacity, PrivacyPolicy visibility, LobbyName name)
            => CreateValid(new LobbyConfiguration(name, capacity, visibility, [], [], []));

        public static Lobby Create(LobbyConfiguration configuration) => CreateValid(configuration);

        private static Lobby CreateValid(LobbyConfiguration configuration)
        {
            if (configuration.Capacity < 4 || int.IsNegative(configuration.Capacity))
                throw new ArgumentException("Capacity must be at least 4.");
            if (configuration.Players.Count() + configuration.Bots.Count() > configuration.Capacity)
                throw new ArgumentException("Total participants exceed lobby capacity.");

            return new Lobby
            {
                PublicId = Lobby.Id.New(),
                InviteCode = InviteCode.New(),
                Name = configuration.Name,
                Capacity = configuration.Capacity,
                PrivacyPolicy = configuration.Visibility,
                Participants = [.. configuration.Players, .. configuration.Bots],
                Packs = []
            };
        }
    }
}


public static partial class LobbyExtensions
{
    extension(Lobby lobby)
    {
        public Player WithHost(AccountId userId, string nickname)
        {
            var host = Player.New(userId, lobby.PublicId, nickname, Role.Host);
            lobby.Participants.Add(host);
            return host;
        }

        public bool IsHost(Player player) => lobby.Host.UserId == player.UserId;
        public bool IsHost(Player.Id id) => lobby.Host.PublicId == id;

        public bool InGame => lobby.State == LobbyState.InGame;
    }
}
