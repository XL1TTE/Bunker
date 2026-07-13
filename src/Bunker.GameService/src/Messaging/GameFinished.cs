namespace Bunker.GameService.Messages;

public record GameFinished(
    Guid LobbyId,
    Guid GameId
);