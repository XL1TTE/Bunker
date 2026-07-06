namespace Bunker.LobbyService.Features.JoinLobby;

public readonly record struct JoinByInviteCode(string InviteCode, string CallerId, string Nickname);

public readonly record struct JoinByPassword(Guid LobbyId, string? Password, string CallerId, string Nickname);

public static class JoinLobby
{
    public abstract record Result
    {
        public record Success(Domain.Lobby Lobby) : Result;
        public record Failure(string Error) : Result;
    }

    public static Result.Success Success(Domain.Lobby lobby) => new Result.Success(lobby);
    public static Result.Failure Failure(string error) => new Result.Failure(error);
}