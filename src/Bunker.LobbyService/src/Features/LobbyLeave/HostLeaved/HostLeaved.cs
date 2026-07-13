namespace Bunker.LobbyService.Features.LeaveLobby.Events;

public readonly record struct HostLeaved(string LobbyId, string HostId);
