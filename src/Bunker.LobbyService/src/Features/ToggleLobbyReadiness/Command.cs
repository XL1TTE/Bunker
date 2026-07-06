namespace Bunker.LobbyService.Features.ToggleLobbyReadiness;

public readonly record struct ToggleLobbyReadiness(string LobbyId, string CallerId)
{
    public abstract record Result
    {
        public record Success(Domain.Lobby Lobby) : Result;
        public record Failure(string Error) : Result;
    }

    public static Result.Success Success(Domain.Lobby lobby) => new Result.Success(lobby);
    public static Result.Failure Failure(string error) => new Result.Failure(error);
}