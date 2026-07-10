namespace Bunker.GameService.Features.CastVote;

public readonly record struct CastVote(Guid GameId, string AccountId, string TargetParticipantId)
{
    public abstract record Result
    {
        public record Success : Result;
        public record Failure(string Error) : Result;
    }

    public static Result.Success Success() => new();
    public static Result.Failure Failure(string error) => new(error);
}