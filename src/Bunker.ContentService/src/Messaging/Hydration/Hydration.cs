using Bunker.ContentService.Transfers;

namespace Bunker.ContentService.Messaging.Hydration;

public record HydrationRequested(IEnumerable<Guid> CardPackIds, IEnumerable<Guid> PersonalityPresetIds);

public record GameContentHydrated(
    IEnumerable<Transfer.CardPack> CardPacks,
    IEnumerable<Transfer.PersonalityPreset> PersonalityPresets,
    IEnumerable<Transfer.ProfessionCard> ProfessionCards,
    IEnumerable<Transfer.HobbiesCard> HobbiesCards,
    IEnumerable<Transfer.AgeCard> AgeCards,
    IEnumerable<Transfer.SexCard> SexCards,
    IEnumerable<Transfer.FactCard> FactCards
);

public record GameContentHydrationFailed(string Reason, IEnumerable<Guid> MissingIds);
