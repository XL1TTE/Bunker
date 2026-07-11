namespace Bunker.LobbyService.Features.UpdateLobbySettings;

public readonly record struct UpdateLobbySettings(
    string LobbyId,
    string CallerId,
    int? Capacity,
    bool? IsPublic,
    string? Password,
    string[]? SelectedPackIds,
    string? Name
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