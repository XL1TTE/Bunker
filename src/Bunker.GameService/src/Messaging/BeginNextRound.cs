using Wolverine.Persistence.Sagas;

namespace Bunker.GameService.Messages;

public record BeginNextRound(
    [property: SagaIdentity] Guid GameId
);