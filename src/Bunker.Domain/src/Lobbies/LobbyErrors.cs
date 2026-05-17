namespace Bunker.Domain.Lobbies;

public static class LobbyErrors
{
    public abstract record AddPlayerError;
    public record PlayerAlreadyInLobby() : AddPlayerError;
    public record LobbyIsFull() : AddPlayerError;

    public abstract record UpdateSettingsError;
    public record CapacityTooSmall() : UpdateSettingsError;

    public abstract record ToggleReadyError;
    public record PlayerNotInLobby() : ToggleReadyError;

    public abstract record RemovePlayerError;
    public record PlayerNotFound() : RemovePlayerError;
}
