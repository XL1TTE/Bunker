using Wolverine.Persistence.Sagas;

namespace Bunker.GameService.Messages;

public record BeginReveal(
    [property: SagaIdentity] Guid GameId
);