using Bunker.ContentService.Transfers;

namespace Bunker.ContentService.Messaging.Hydration;

public record RequestGameContentHydration(
    Guid StartRequestId,
    IReadOnlyList<Guid> CardPackIds,
    IReadOnlyList<Guid> PersonalityPresetIds
);

public record GameContentHydrated(
    Guid StartRequestId,
    IReadOnlyList<Transfer.CardPack> CardPacks,
    IReadOnlyList<Transfer.PersonalityPreset> PersonalityPresets,
    IReadOnlyList<Transfer.ProfessionCard> ProfessionCards,
    IReadOnlyList<Transfer.HobbiesCard> HobbiesCards,
    IReadOnlyList<Transfer.AgeCard> AgeCards,
    IReadOnlyList<Transfer.SexCard> SexCards,
    IReadOnlyList<Transfer.FactCard> FactCards
);

public record GameContentHydrationFailed(
    Guid StartRequestId,
    string Reason,
    IReadOnlyList<Guid> MissingIds
);
