namespace Bunker.LobbyService.Messages;

public record PlayerLeftGame(
    Guid LobbyId,
    Guid GameId,
    string AccountId
);