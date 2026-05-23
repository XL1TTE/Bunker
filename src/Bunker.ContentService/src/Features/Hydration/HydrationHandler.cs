using Bunker.ContentService.Messages;
using Bunker.ContentService.Persistence;
using Bunker.ContentService.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Wolverine;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Hydration;

[WolverineHandler]
public static class HydrationHandler
{
    public static async Task Handle(HydrateGameContent command, ContentDbContext db, IMessageBus bus)
    {
        var packIds = command.CardPackIds.Distinct().ToList();
        var personalityIds = command.PersonalityPresetIds.Distinct().ToList();

        var packs = await db.CardPacks
            .Include(x => x.Cards)
            .Where(x => packIds.Contains(x.PublicId))
            .ToListAsync();

        var personalities = await db.PersonalityPresets
            .Where(x => personalityIds.Contains(x.PublicId))
            .ToListAsync();

        // Strict Validation
        var missingPacks = packIds.Except(packs.Select(x => x.PublicId)).ToList();
        var missingPersonalities = personalityIds.Except(personalities.Select(x => x.PublicId)).ToList();

        if (missingPacks.Any() || missingPersonalities.Any())
        {
            var missingIds = missingPacks.Concat(missingPersonalities).ToList();
            await bus.PublishAsync(new GameContentHydrationFailed("Some requested content IDs are missing.", missingIds));
            return;
        }

        // Fetch all cards for the packs
        var allCardIds = packs.SelectMany(x => x.Cards).Select(x => x.CardId).Distinct().ToList();
        var allCards = await db.Cards
            .Where(x => allCardIds.Contains(x.PublicId))
            .ToListAsync();

        // Map to events
        var professionCards = allCards.OfType<ProfessionCard>().Select(x => new ProfessionCardUpdated(x.PublicId, x.Profession));
        var hobbiesCards = allCards.OfType<HobbiesCard>().Select(x => new HobbiesCardUpdated(x.PublicId, x.Hobbies));
        var ageCards = allCards.OfType<AgeCard>().Select(x => new AgeCardUpdated(x.PublicId, x.Age));
        var sexCards = allCards.OfType<SexCard>().Select(x => new SexCardUpdated(x.PublicId, (byte)x.Sex));
        var factCards = allCards.OfType<FactCard>().Select(x => new FactCardUpdated(x.PublicId, x.Fact));

        var packEvents = packs.Select(x => new CardPackUpdated(x.PublicId, x.Title, x.Description, x.Cards.Select(c => c.CardId)));
        var personalityEvents = personalities.Select(x => new PersonalityPresetUpdated(x.PublicId, x.Title, x.Description));

        await bus.PublishAsync(new GameContentHydrated(
            packEvents,
            personalityEvents,
            professionCards,
            hobbiesCards,
            ageCards,
            sexCards,
            factCards
        ));
    }
}
