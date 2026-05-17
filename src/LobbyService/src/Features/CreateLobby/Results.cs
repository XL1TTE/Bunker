namespace LobbyService.Features.CreateLobby;

public abstract record CreateLobbyResult;
public record CreateLobbySuccess(CreateLobbyResponse Response) : CreateLobbyResult;
public record CreateLobbyUnauthorized : CreateLobbyResult;
public record CreateLobbyFailure(string Reason) : CreateLobbyResult;

public static class CreateLobbyResultFactory
{
    extension (CreateLobbyResult)
    {
        public static CreateLobbyResult Success(CreateLobbyResponse response) => new CreateLobbySuccess(response);
        public static CreateLobbyResult Unauthorized() => new CreateLobbyUnauthorized();
        public static CreateLobbyResult Failure(string reason) => new CreateLobbyFailure(reason);
    }
}
