namespace Bunker.LobbyService.Messages;

public record GameStartFailed(
    Guid StartRequestId,
    Guid LobbyId,
    string Reason
);