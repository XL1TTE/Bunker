namespace Bunker.LobbyService.Messages;

public record GameFinished(
    Guid LobbyId,
    Guid GameId
);