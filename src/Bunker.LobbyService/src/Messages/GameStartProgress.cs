namespace Bunker.LobbyService.Messages;

public record GameStartProgress(
    Guid StartRequestId,
    Guid LobbyId,
    string Step,
    string Status,
    string? Message
);