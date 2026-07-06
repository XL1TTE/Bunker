using Wolverine.Persistence.Sagas;

namespace Bunker.GameService.Messages;

public record GameContentHydrationFailed(
    [property: SagaIdentity] Guid StartRequestId,
    string Reason,
    IReadOnlyList<Guid> MissingIds
);