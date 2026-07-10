namespace Bunker.GameService.Features.RevealAttribute;

public readonly record struct RevealAttribute(
    Guid GameId,
    string AccountId,
    string AttributeKind
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