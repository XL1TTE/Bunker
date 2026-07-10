using Wolverine.Persistence.Sagas;

namespace Bunker.GameService.Messages;

public record BeginDiscussion(
    [property: SagaIdentity] Guid GameId
);