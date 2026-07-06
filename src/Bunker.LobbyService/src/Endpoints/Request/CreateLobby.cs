namespace Bunker.LobbyService.Api.Endpoints.Request;

public readonly record struct CreateLobby(
    int Capacity,
    bool IsPublic,
    string[] SelectedPackIds,
    string? Password
);