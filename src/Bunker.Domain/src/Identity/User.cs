using static Bunker.Domain.Identity.User;

namespace Bunker.Domain.Identity;

public class User
{
    public readonly record struct Id(Guid Value)
    {
        public static Id Empty { get; } = new(Guid.Empty);
        public static Id New() => new(Guid.NewGuid());
        public static Id Restore(Guid value) => new(value);
    }

    private User() { } // EF Core

    internal User(Id id, Nickname nickname, Stats stats)
    {
        PublicId = id;
        Nickname = nickname;
        Stats    = stats;
    }

    public Id PublicId { get; private set; }
    public Nickname Nickname    { get; private set; }
    public Stats    Stats       { get; private set; }

    public void UpdateNickname(Nickname nickname) => Nickname = nickname;
}

public static class PlayerFactory
{
    extension(User)
    {
        public static User Create(Nickname nickname) => new User(Id.New(), nickname, Stats.Create());
        public static User Create(Id id, Nickname nickname) => new User(id, nickname, Stats.Create());
        public static User Restore(Id id, Nickname nickname, Stats stats) => new User(id, nickname, stats);
    }
}
