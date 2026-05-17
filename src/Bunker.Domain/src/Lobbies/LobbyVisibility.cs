namespace Bunker.Domain.Lobbies;

public abstract record LobbyVisibility
{
    public static LobbyVisibility Public  { get; } = new PublicLobby();
    public static LobbyVisibility Private { get; } = new PrivateLobby();
}

public record PublicLobby  : LobbyVisibility;
public record PrivateLobby : LobbyVisibility;
