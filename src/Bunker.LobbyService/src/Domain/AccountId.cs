namespace Bunker.LobbyService.Domain;

public readonly record struct AccountId(string Value)
{
    public static AccountId Empty { get; } = new(Guid.Empty.ToString());
    public static AccountId New() => new(Guid.NewGuid().ToString());
    public static AccountId Create(string value) => new(value);
}

public sealed record AccountReadModel(AccountId PublicId);
