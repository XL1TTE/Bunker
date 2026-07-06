namespace Bunker.LobbyService.Messages;

public record GameStartRequested(
    Guid StartRequestId,
    Guid LobbyId,
    string HostId,
    IReadOnlyList<Guid> CardPackIds,
    IReadOnlyList<Guid> PersonalityPresetIds,
    IReadOnlyList<SagaParticipant> Participants
);