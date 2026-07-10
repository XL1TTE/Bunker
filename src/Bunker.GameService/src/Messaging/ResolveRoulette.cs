using Wolverine.Persistence.Sagas;

namespace Bunker.GameService.Messages;

public record ResolveRoulette(
    [property: SagaIdentity] Guid GameId
);