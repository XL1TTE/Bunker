using Bunker.ContentService.Messages;
using Bunker.ContentService.Messaging.Hydration;
using Bunker.ContentService.Persistence.Contracts;
using Bunker.ContentService.Transfers;
using Bunker.ContentService.Domain;
using Wolverine;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Hydration;

[WolverineHandler]
public static class HydrationHandler
{
    public static async Task Handle(HydrationRequested command, IHydrationQueries queries, IMessageBus bus)
    {
        var packIds = command.CardPackIds.Distinct().ToList();
        var personalityIds = command.PersonalityPresetIds.Distinct().ToList();

        var (packs, personalities, allCards) = await queries.GetHydrationDataAsync(packIds, personalityIds);

        // Strict Validation
        var missingPacks = packIds.Except(packs.Select(x => x.PublicId.Value)).ToList();
        var missingPersonalities = personalityIds.Except(personalities.Select(x => x.PublicId.Value)).ToList();

        if (missingPacks.Any() || missingPersonalities.Any())
        {
            var missingIds = missingPacks.Concat(missingPersonalities).ToList();
            await bus.PublishAsync(new GameContentHydrationFailed("Some requested content IDs are missing.", missingIds));
            return;
        }

        // Map to events
        var professionCards = allCards.OfType<ProfessionCard>().Select(x => new Transfer.ProfessionCard(x.PublicId.Value, x.Profession));
        var hobbiesCards = allCards.OfType<HobbiesCard>().Select(x => new Transfer.HobbiesCard(x.PublicId.Value, x.Hobbies));
        var ageCards = allCards.OfType<AgeCard>().Select(x => new Transfer.AgeCard(x.PublicId.Value, x.Age));
        var sexCards = allCards.OfType<SexCard>().Select(x => new Transfer.SexCard(x.PublicId.Value, x.Sex.ToString()));
        var factCards = allCards.OfType<FactCard>().Select(x => new Transfer.FactCard(x.PublicId.Value, x.Fact));

        var packEvents = packs.Select(x => new Transfer.CardPack(x.PublicId.Value.ToString(), x.Title, x.Description, x.Cards.Select(c => c.CardId.Value).ToList()));
        var personalityEvents = personalities.Select(x => new Transfer.PersonalityPreset(x.PublicId.Value, x.Title, x.Description));

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
