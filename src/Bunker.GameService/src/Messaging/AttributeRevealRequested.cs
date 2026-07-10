using Wolverine.Persistence.Sagas;

namespace Bunker.GameService.Messages;

public record AttributeRevealRequested(
    [property: SagaIdentity] Guid GameId,
    string ParticipantId,
    string AttributeKind
);