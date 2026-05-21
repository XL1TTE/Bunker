namespace Bunker.AccountService.Domain;

public class Account
{
    public readonly record struct Id(Guid Value)
    {
        public static Id Empty { get; } = new(Guid.Empty);
        public static Id New() => new(Guid.NewGuid());
        public static Id Create(Guid value) => new(value);
    }

    private Account() { } // EF Core

    internal Account(Id id, Nickname nickname, Stats stats)
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
    extension(Account)
    {
        public static Account Create(Nickname nickname) => new Account(Account.Id.New(), nickname, Stats.Create());
        public static Account Create(Account.Id id, Nickname nickname) => new Account(id, nickname, Stats.Create());
        public static Account Create(Account.Id id, Nickname nickname, Stats stats) => new Account(id, nickname, stats);
    }
}
