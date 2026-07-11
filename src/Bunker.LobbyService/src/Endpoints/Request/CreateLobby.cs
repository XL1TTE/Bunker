namespace Bunker.LobbyService.Api.Endpoints.Request;

public readonly record struct CreateLobby(
    string Name,
    int Capacity,
    bool IsPublic,
    string[] SelectedPackIds,
    string? Password
);