namespace Bunker.LobbyService.Features.CreateLobby;

public readonly record struct CreateLobby(
    string HostId,
    string Nickname,
    int Capacity,
    bool Visible,
    string LobbyPassword,
    string[] CardPackIds
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
