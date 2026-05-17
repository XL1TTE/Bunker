namespace LobbyService.Features.ToggleReady;

public abstract record ToggleReadyResult;
public record ToggleReadySuccess : ToggleReadyResult;
public record ToggleReadyNotFound : ToggleReadyResult;
public record ToggleReadyPlayerNotInLobby : ToggleReadyResult;
public record ToggleReadyUnauthorized : ToggleReadyResult;

public static class ToggleReadyResultFactory
{
    extension (ToggleReadyResult)
    {
        public static ToggleReadyResult Success() => new ToggleReadySuccess();
        public static ToggleReadyResult NotFound() => new ToggleReadyNotFound();
        public static ToggleReadyResult PlayerNotInLobby() => new ToggleReadyPlayerNotInLobby();
        public static ToggleReadyResult Unauthorized() => new ToggleReadyUnauthorized();
    }
}
