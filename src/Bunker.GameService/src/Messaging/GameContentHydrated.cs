using Wolverine.Persistence.Sagas;

namespace Bunker.GameService.Messages;

public record GameContentHydrated([property: SagaIdentity] Guid StartRequestId);