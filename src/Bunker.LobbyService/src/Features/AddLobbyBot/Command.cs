namespace Bunker.LobbyService.Features.AddLobbyBot;

public readonly record struct AddLobbyBot(
    string LobbyId,
    string CallerId,
    Guid PersonalityPresetId,
    string Nickname
)
{
    public abstract record Result
    {
        public record Success(Domain.Lobby Lobby) : Result;
        public record Failure(string Error) : Result;
    }

    public static Result.Success Success(Domain.Lobby lobby) => new Result.Success(lobby);
    public static Result.Failure Failure(string error) => new Result.Failure(error);
}