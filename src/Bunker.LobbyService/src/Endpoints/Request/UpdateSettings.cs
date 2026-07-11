namespace Bunker.LobbyService.Api.Endpoints.Request;

public readonly record struct UpdateSettingsRequest(
    int? Capacity,
    bool? IsPublic,
    string[]? SelectedPackIds,
    string? Password,
    string? Name
);