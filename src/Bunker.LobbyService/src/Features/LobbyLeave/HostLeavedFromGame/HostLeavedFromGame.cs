namespace Bunker.LobbyService.Features.LeaveLobby.Events;

public readonly record struct HostLeavedFromGame(string? GameId, string LobbyId, string HostId);
