using Wolverine.Persistence.Sagas;

namespace Bunker.GameService.Messages;

public record LeaveRequested(
    [property: SagaIdentity] Guid GameId,
    string ParticipantId
);