namespace Bunker.LobbyService.Features.LeaveLobby.Events;

public readonly record struct PlayerLeaved(string LobbyId, string PlayerId);
