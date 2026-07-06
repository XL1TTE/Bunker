namespace Bunker.LobbyService.Features.RemoveLobbyBot;

public readonly record struct RemoveLobbyBot(
    string LobbyId,
    string CallerId,
    Guid ParticipantId
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