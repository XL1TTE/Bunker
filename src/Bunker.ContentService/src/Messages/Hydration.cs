namespace Bunker.ContentService.Messages;

public record HydrateGameContent(IEnumerable<Guid> CardPackIds, IEnumerable<Guid> PersonalityPresetIds);

public record GameContentHydrated(
    IEnumerable<CardPackUpdated> CardPacks,
    IEnumerable<PersonalityPresetUpdated> PersonalityPresets,
    IEnumerable<ProfessionCardUpdated> ProfessionCards,
    IEnumerable<HobbiesCardUpdated> HobbiesCards,
    IEnumerable<AgeCardUpdated> AgeCards,
    IEnumerable<SexCardUpdated> SexCards,
    IEnumerable<FactCardUpdated> FactCards
);

public record GameContentHydrationFailed(string Reason, IEnumerable<Guid> MissingIds);
