namespace Bunker.LobbyService.Features.CreateLobby;

public readonly record struct CreateLobby(
    string HostId,
    string Nickname,
    string Name,
    int Capacity,
    bool IsPublic,
    string? Password,
    string[] SelectedPackIds
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