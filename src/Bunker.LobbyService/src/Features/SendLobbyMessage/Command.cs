using Bunker.LobbyService.Transfers;

namespace Bunker.LobbyService.Features.SendLobbyMessage;

public readonly record struct SendLobbyMessage(string LobbyId, string CallerId, string Text)
{
    public abstract record Result
    {
        public record Success(Transfer.ChatMessage Message) : Result;
        public record Failure(string Error) : Result;
    }

    public static Result.Success Success(Transfer.ChatMessage message) => new Result.Success(message);
    public static Result.Failure Failure(string error) => new Result.Failure(error);
}