namespace Bunker.LobbyService.Features.LeaveLobby;

public readonly record struct LeaveLobby(string LobbyId, string CallerId)
{
    public abstract record Result
    {
        public record Success : Result;
        public record Failure(string Error) : Result;
    }

    public static Result Success() => new Result.Success();
    public static Result Failure(string error) => new Result.Failure(error);
}