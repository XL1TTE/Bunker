using System.Collections.ObjectModel;
using Bunker.Domain.Bots;
using static Bunker.Domain.Cards.CardPack;

namespace Bunker.Domain.Lobbies;

public class Lobby
{
    public readonly record struct LobbyId(Guid Value)
    {
        public static LobbyId Empty { get; }      =  new LobbyId(Guid.Empty);
        public static LobbyId New()               => new LobbyId(Guid.NewGuid());
        public static LobbyId Restore(Guid value) => new LobbyId(value);
    }
    
    internal Lobby(LobbyConfiguration configuration)
    {
        PublicId    = LobbyId.New();
        InviteCode  = InviteCode.New();
        Capacity    = configuration.Capacity;
        Visibility  = configuration.Visibility;
        Players     = new Collection<Player>(configuration.Players.ToList());
        Bots        = new Collection<Bot>(configuration.Bots.ToList());
        Packs       = new Collection<CardPackId>(configuration.CardPacks.ToList());
    }
    
    private Lobby() { } // EF Core

    public LobbyId PublicId      { get; internal set; }
    public InviteCode InviteCode { get; internal set; }
    public int Capacity          { get; internal set; }

    public LobbyVisibility Visibility { get; internal set; } = LobbyVisibility.Public;
    
    public Collection<Player> Players   { get; internal set; } = [];
    public Collection<Bot> Bots         { get; internal set; } = [];
    public Collection<CardPackId> Packs { get; internal set; } = [];
}

public readonly record struct LobbyConfiguration(
    int Capacity,
    LobbyVisibility Visibility,
    IReadOnlyCollection<Player> Players,
    IReadOnlyCollection<Bot> Bots,
    IReadOnlyCollection<CardPackId> CardPacks);

public static class LobbyFactory
{       
    extension (Lobby)
    {
        public static Lobby Create(LobbyConfiguration configuration) => CreateValid(configuration);

        private static Lobby CreateValid(LobbyConfiguration configuration)
        {
            if (configuration.Capacity < 4)
                throw new ArgumentException("Capacity must be at least 4.");
            if (configuration.Players.Count() + configuration.Bots.Count() > configuration.Capacity)
                throw new ArgumentException("Total participants exceed lobby capacity.");
            
            return new Lobby(configuration);
        }
    }
}
