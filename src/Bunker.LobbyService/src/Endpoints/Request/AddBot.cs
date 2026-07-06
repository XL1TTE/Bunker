namespace Bunker.LobbyService.Api.Endpoints.Request;

public readonly record struct AddBotRequest(
    Guid PersonalityPresetId,
    string Nickname
);