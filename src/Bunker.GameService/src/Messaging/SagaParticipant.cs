namespace Bunker.GameService.Messages;

public record SagaParticipant(
    string Id,
    string Nickname,
    string Type,
    Guid? PersonalityPresetId
);