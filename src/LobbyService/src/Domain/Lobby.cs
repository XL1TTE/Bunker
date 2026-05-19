using System.Collections.ObjectModel;
using Bunker.Domain.Identity;

namespace Bunker.LobbyService.Domain;

public record Lobby
{
    public readonly record struct Id(Guid Value)
    {
        public static Id Empty { get; } = new(Guid.Empty);
        public static Id New() => new(Guid.NewGuid());
        public static Id Restore(Guid value) => new(value);
    }

    internal Lobby() { } // EF Core

    public Id PublicId { get; internal set; }
    public InviteCode InviteCode { get; internal set; }
    public int Capacity          { get; internal set; }

    public PrivacyPolicy PrivacyPolicy { get; internal set; } = PrivacyPolicy.PublicPolicy();

    public Collection<LobbyParticipant> Participants { get; internal set; } = [];
    public Collection<LobbyCardPack> Packs { get; internal set; } = [];

    public IReadOnlyCollection<PlayerParticipant> Players => Participants.OfType<PlayerParticipant>().ToList().AsReadOnly();
    public IReadOnlyCollection<BotParticipant> Bots => Participants.OfType<BotParticipant>().ToList().AsReadOnly();
}

public readonly record struct LobbyConfiguration(
    int Capacity,
    PrivacyPolicy Visibility,
    IReadOnlyCollection<PlayerParticipant> Players,
    IReadOnlyCollection<BotParticipant> Bots,
    IReadOnlyCollection<LobbyCardPack> CardPacks);

public static class LobbyFactory
{       
    extension (Lobby)
    {
        public static Lobby Create(int capacity, PrivacyPolicy visibility)
            => CreateValid(new LobbyConfiguration(capacity, visibility, [], [], []));

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
        public PlayerParticipant WithHost(User.Id userId, string nickname)
        {
            var host = PlayerParticipantFactory.New(userId, lobby.PublicId, nickname, Role.Host);
            lobby.Participants.Add(host);
            return host;
        }
    }
}
