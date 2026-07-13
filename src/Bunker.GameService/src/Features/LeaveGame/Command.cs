namespace Bunker.GameService.Features.LeaveGame;

public readonly record struct LeaveGame(Guid GameId, string AccountId)
{
    public abstract record Result
    {
        public record Success : Result;
        public record Failure(string Error) : Result;
    }

    public static Result.Success Success() => new();
    public static Result.Failure Failure(string error) => new(error);
}