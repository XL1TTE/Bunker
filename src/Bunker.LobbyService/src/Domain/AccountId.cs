namespace Bunker.LobbyService.Domain;

public readonly record struct AccountId(Guid Value)
{
    public static AccountId Empty { get; } = new(Guid.Empty);
    public static AccountId New() => new(Guid.NewGuid());
    public static AccountId Create(Guid value) => new(value);
}
