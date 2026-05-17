using static Bunker.Domain.Identity.User;

namespace Bunker.Domain.Identity;

public class User
{
    public readonly record struct UserId(Guid Value)
    {
        public static UserId Empty { get; }      = new UserId(Guid.Empty);
        public static UserId New()               => new UserId(Guid.NewGuid());
        public static UserId Restore(Guid value) => new UserId(value);
    }

    private User() { } // EF Core

    internal User(UserId id, Nickname nickname, Stats stats)
    {
        PublicId = id;
        Nickname = nickname;
        Stats    = stats;
    }
    
    public UserId   PublicId    { get; private set; } 
    public Nickname Nickname    { get; private set; }
    public Stats    Stats       { get; private set; }

    public void UpdateNickname(Nickname nickname) => Nickname = nickname;
}

public static class PlayerFactory
{
    extension(User)
    {
        public static User Create(Nickname nickname) => new User(UserId.New(), nickname, Stats.Create());
        public static User Restore(UserId id, Nickname nickname, Stats stats) => new User(id, nickname, stats);
    }
}
