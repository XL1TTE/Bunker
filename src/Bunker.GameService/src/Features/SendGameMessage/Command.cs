using Bunker.GameService.Transfers;

namespace Bunker.GameService.Features.SendGameMessage;

public readonly record struct SendGameMessage(Guid GameId, string AccountId, string Text)
{
    public abstract record Result
    {
        public record Success(ChatMessageDto Message) : Result;
        public record Failure(string Error) : Result;
    }

    public static Result.Success Success(ChatMessageDto message) => new(message);
    public static Result.Failure Failure(string error) => new(error);
}