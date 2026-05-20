namespace Bunker.LobbyService.Domain;

public readonly record struct BotPersonalityId(Guid Value)
{
    public static BotPersonalityId Create(Guid value) => new(value);
}
    