using Wolverine.Persistence.Sagas;

namespace Bunker.GameService.Messages;

public record BeginVoting(
    [property: SagaIdentity] Guid GameId
);