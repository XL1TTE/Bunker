using Wolverine.Persistence.Sagas;

namespace Bunker.GameService.Messages;

public record VoteRequested(
    [property: SagaIdentity] Guid GameId,
    string VoterId,
    string TargetId
);