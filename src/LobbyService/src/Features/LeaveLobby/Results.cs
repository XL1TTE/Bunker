namespace LobbyService.Features.LeaveLobby;

public abstract record LeaveLobbyResult;
public record LeaveLobbySuccess : LeaveLobbyResult;
public record LeaveLobbyNotFound : LeaveLobbyResult;
public record LeaveLobbyUnauthorized : LeaveLobbyResult;

public static class LeaveLobbyResultFactory
{
    extension (LeaveLobbyResult)
    {
        public static LeaveLobbyResult Success() => new LeaveLobbySuccess();
        public static LeaveLobbyResult NotFound() => new LeaveLobbyNotFound();
        public static LeaveLobbyResult Unauthorized() => new LeaveLobbyUnauthorized();
    }
}
