using Wolverine.Persistence.Sagas;

namespace Bunker.GameService.Messages;

public record BeginFinish(
    [property: SagaIdentity] Guid GameId
);