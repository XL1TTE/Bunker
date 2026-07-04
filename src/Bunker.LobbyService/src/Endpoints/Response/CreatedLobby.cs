using Bunker.LobbyService.Transfers;

namespace Bunker.LobbyService.Api.Endpoints.Responses;

/// <summary>
/// Response with created lobby snapshot.
/// </summary>
/// <param name="LobbySnapshot">Lobby snapshot.</param>
public readonly record struct CreatedLobby(
    Transfer.LobbySnapshot LobbySnapshot    
);
