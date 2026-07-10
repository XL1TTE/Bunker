using Wolverine.Persistence.Sagas;

namespace Bunker.GameService.Messages;

public record AdvanceTurn(
    [property: SagaIdentity] Guid GameId
);