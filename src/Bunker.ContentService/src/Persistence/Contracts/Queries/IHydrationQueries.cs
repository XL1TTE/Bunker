using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Persistence.Contracts;

public interface IHydrationQueries
{
    Task<(IReadOnlyCollection<CardPack> Packs, IReadOnlyCollection<PersonalityPreset> Presets, IReadOnlyCollection<Card> Cards)> GetHydrationDataAsync(
        IEnumerable<Guid> cardPackIds, 
        IEnumerable<Guid> personalityPresetIds);
}
