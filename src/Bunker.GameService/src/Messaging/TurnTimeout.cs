using Wolverine.Persistence.Sagas;

namespace Bunker.GameService.Messages;

public record TurnTimeout(
    [property: SagaIdentity] Guid GameId,
    string Phase,
    int RoundNumber,
    int TurnIndex,
    int Epoch
);