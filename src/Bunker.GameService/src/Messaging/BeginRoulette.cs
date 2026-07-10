using Wolverine.Persistence.Sagas;

namespace Bunker.GameService.Messages;

public record BeginRoulette(
    [property: SagaIdentity] Guid GameId
);