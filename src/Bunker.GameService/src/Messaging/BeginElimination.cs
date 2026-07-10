using Wolverine.Persistence.Sagas;

namespace Bunker.GameService.Messages;

public record BeginElimination(
    [property: SagaIdentity] Guid GameId
);