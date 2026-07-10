namespace Bunker.LobbyService.Messages;

public record SagaParticipant(
    string Id,
    string Nickname,
    string Type,
    Guid? PersonalityPresetId,
    string? AccountId
);