namespace Bunker.LobbyService.Api.Endpoints.Request;

public readonly record struct JoinByPasswordRequest(
    string? Password
);