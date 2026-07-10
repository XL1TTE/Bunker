using Wolverine.Persistence.Sagas;

namespace Bunker.GameService.Messages;

public record BeginIntroDiscussion(
    [property: SagaIdentity] Guid GameId
);