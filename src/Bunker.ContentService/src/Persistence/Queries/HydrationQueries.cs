using Bunker.ContentService.Persistence.Contracts;
using Bunker.ContentService.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Bunker.ContentService.Persistence.Queries;

public class DbContextHydrationQueries(ContentDbContext db) : IHydrationQueries
{
    public async Task<(IReadOnlyCollection<Domain.CardPack> Packs, IReadOnlyCollection<Domain.PersonalityPreset> Presets, IReadOnlyCollection<Domain.Card> Cards)> GetHydrationDataAsync(
        IEnumerable<Guid> cardPackIds, 
        IEnumerable<Guid> personalityPresetIds)
    {
        var packs = await db.CardPacks.AsNoTracking()
            .Include(x => x.Cards)
            .Where(x => cardPackIds.Contains(x.PublicId))
            .ToListAsync();

        var personalities = await db.PersonalityPresets.AsNoTracking()
            .Where(x => personalityPresetIds.Contains(x.PublicId))
            .ToListAsync();

        var cardIds = packs.SelectMany(x => x.Cards).Select(x => x.CardId).Distinct().ToList();
        var cards = await db.Cards.AsNoTracking()
            .Where(x => cardIds.Contains(x.PublicId))
            .ToListAsync();

        return (
            packs.Select(x => x.ToDomain()).ToList(),
            personalities.Select(x => x.ToDomain()).ToList(),
            cards.Select(x => x.ToDomain()).ToList()
        );
    }
}
