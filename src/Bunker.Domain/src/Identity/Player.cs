using Bunker.Domain.Primitives;

namespace Bunker.Domain.Identity;

public class Player : Entity<PlayerId>
{
    private Player() : base(PlayerId.Create()) {} // EF Core
    
    internal Player(PlayerId id, Nickname nickname, Stats stats) : base(id)
    {
        Nickname = nickname;
        Stats = stats;
    }

    public Nickname Nickname { get; private set; }
    public Stats Stats { get; private set; }

    public void UpdateNickname(Nickname newNickname)
    {
        Nickname = newNickname;
    }
}

public static class PlayerFactory
{
    extension(Player)
    {
        public static Player Create(PlayerId id, Nickname nickname)
        {
            return new Player(id, nickname, Stats.Create());
        }

        public static Player Restore(PlayerId id, Nickname nickname, Stats stats)
        {
            return new Player(id, nickname, stats);
        }
    }
}
