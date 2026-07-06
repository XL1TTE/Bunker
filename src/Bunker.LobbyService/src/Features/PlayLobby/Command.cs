namespace Bunker.LobbyService.Features.PlayLobby;

public readonly record struct PlayLobby(
    string LobbyId,
    string HostId
)
{
    public abstract record Result
    {
        public record Success : Result;
        public record Failure(string Error) : Result;
    }

    public static Result.Success Success() => new Result.Success();
    public static Result.Failure Failure(string error) => new Result.Failure(error);
}