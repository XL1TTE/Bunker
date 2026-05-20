namespace Bunker.LobbyService.Domain;

public readonly record struct CardPackId(Guid Value)
{
    public static CardPackId Create(Guid value) => new(value);
}
    