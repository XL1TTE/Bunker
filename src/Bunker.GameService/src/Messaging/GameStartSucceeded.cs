namespace Bunker.GameService.Messages;

public record GameStartSucceeded(
    Guid StartRequestId,
    Guid LobbyId,
    Guid GameId,
    string JoinUrl
);