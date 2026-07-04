namespace Bunker.LobbyService.Api.Endpoints.Request;

/// <summary>
/// Request to create a lobby.
/// </summary>
/// <param name="Capacity">Capacity of the lobby.</param>
/// <param name="Visible">Indicates if the lobby is visible in lobby browser.</param>
/// <param name="LobbyPassword">Can be empty.</param>
/// <param name="CardPackIds">List of card pack IDs for the lobby.</param>
public readonly record struct CreateLobby(
    int Capacity,
    bool Visible,
    string LobbyPassword,
    string[] CardPackIds
);
