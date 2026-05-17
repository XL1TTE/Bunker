namespace LobbyService.Features.StartGame;

public abstract record StartGameResult;
public record StartGameSuccess : StartGameResult;
public record StartGameNotFound : StartGameResult;
public record StartGameForbidden : StartGameResult;
public record StartGameNotReady : StartGameResult;
public record StartGameUnauthorized : StartGameResult;

public static class StartGameResultFactory
{
    extension (StartGameResult)
    {
        public static StartGameResult Success() => new StartGameSuccess();
        public static StartGameResult NotFound() => new StartGameNotFound();
        public static StartGameResult Forbidden() => new StartGameForbidden();
        public static StartGameResult NotReady() => new StartGameNotReady();
        public static StartGameResult Unauthorized() => new StartGameUnauthorized();
    }
}
