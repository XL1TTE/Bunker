namespace Bunker.GameService.Messages;

public record PlayerLeftGame(
    Guid LobbyId,
    Guid GameId,
    string AccountId
);