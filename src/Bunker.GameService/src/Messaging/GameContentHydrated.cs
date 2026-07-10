using Wolverine.Persistence.Sagas;

namespace Bunker.GameService.Messages;

public record GameContentHydrated(
    [property: SagaIdentity] Guid StartRequestId,
    IReadOnlyList<GameProfessionCard> ProfessionCards,
    IReadOnlyList<GameHobbiesCard> HobbiesCards,
    IReadOnlyList<GameAgeCard> AgeCards,
    IReadOnlyList<GameSexCard> SexCards,
    IReadOnlyList<GameFactCard> FactCards,
    IReadOnlyList<GameHealthCard> HealthCards,
    IReadOnlyList<GameLuggageCard> LuggageCards,
    IReadOnlyList<GameBunkerCard> BunkerCards
);