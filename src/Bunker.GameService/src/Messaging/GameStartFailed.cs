namespace Bunker.GameService.Messages;

public record GameStartFailed(
    Guid StartRequestId,
    Guid LobbyId,
    string Reason
);