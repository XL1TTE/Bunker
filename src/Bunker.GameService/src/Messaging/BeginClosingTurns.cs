using Wolverine.Persistence.Sagas;

namespace Bunker.GameService.Messages;

public record BeginClosingTurns(
    [property: SagaIdentity] Guid GameId
);