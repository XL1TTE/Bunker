namespace Bunker.LobbyService.Api.Endpoints.Request;

public readonly record struct SendMessageRequest(
    string Text
);