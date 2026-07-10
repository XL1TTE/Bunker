using Wolverine.Persistence.Sagas;

namespace Bunker.GameService.Messages;

public record BeginBunkerIntroduction(
    [property: SagaIdentity] Guid GameId
);