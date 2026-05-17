namespace LobbyService.Features.JoinLobby;

/// <summary>
/// Data required to join an existing lobby.
/// </summary>
/// <param name="InviteCode">The 6-character alphanumeric code for the target lobby.</param>
public record JoinLobbyRequest(string InviteCode);

/// <summary>
/// Information about the successfully joined lobby.
/// </summary>
/// <param name="LobbyId">The unique identifier of the lobby that was joined.</param>
public record JoinLobbyResponse(Guid LobbyId);

public abstract record JoinLobbyResult;
public record JoinLobbySuccess(JoinLobbyResponse Response) : JoinLobbyResult;
public record JoinLobbyNotFound : JoinLobbyResult;
public record JoinLobbyFull : JoinLobbyResult;
public record JoinLobbyAlreadyIn : JoinLobbyResult;
public record JoinLobbyUnauthorized : JoinLobbyResult;

public static class JoinLobbyResultFactory
{
    extension (JoinLobbyResult)
    {
        public static JoinLobbyResult Success(JoinLobbyResponse response) => new JoinLobbySuccess(response);
        public static JoinLobbyResult NotFound() => new JoinLobbyNotFound();
        public static JoinLobbyResult Full() => new JoinLobbyFull();
        public static JoinLobbyResult AlreadyIn() => new JoinLobbyAlreadyIn();
        public static JoinLobbyResult Unauthorized() => new JoinLobbyUnauthorized();
    }
}
