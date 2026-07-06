namespace Bunker.LobbyService.Domain;

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

    public abstract record StartGameError;
    public record NotHost() : StartGameError;
    public record NotAllReady() : StartGameError;
    public record InsufficientPlayers() : StartGameError;

    public abstract record LeaveError;
    public record CallerNotInLobby() : LeaveError;

    public abstract record KickError;
    public record KickerNotHost() : KickError;
    public record KickParticipantNotFound() : KickError;
    public record CannotKickHost() : KickError;

    public abstract record RemoveBotError;
    public record BotRemoverNotHost() : RemoveBotError;
    public record BotNotFound() : RemoveBotError;

    public abstract record JoinError;
    public record LobbyNotFound() : JoinError;
    public record WrongPassword() : JoinError;
    public record InvalidInviteCode() : JoinError;
}