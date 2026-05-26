using Bunker.ContentService.Messaging.Hydration;
using Bunker.ContentService.Persistence.Contracts;
using Bunker.ContentService.Transfers;
using Bunker.ContentService.Domain;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Hydration;

[WolverineHandler]
public static class HydrationHandler
{
    public static async IAsyncEnumerable<object> Handle(HydrationRequested command, IHydrationQueries queries)
    {
        var packIds = command.CardPackIds.Distinct().ToList();
        var personalityIds = command.PersonalityPresetIds.Distinct().ToList();

        var (packs, personalities, allCards) = await queries.GetHydrationDataAsync(packIds, personalityIds);

        var missingPacks = packIds.Except(packs.Select(x => x.PublicId.Value)).ToList();
        var missingPersonalities = personalityIds.Except(personalities.Select(x => x.PublicId.Value)).ToList();

        if (missingPacks.Any() || missingPersonalities.Any())
        {
            var missingIds = missingPacks.Concat(missingPersonalities).ToList();
            yield return new GameContentHydrationFailed("Some requested content IDs are missing.", missingIds);
        }

        var professionCards = allCards.OfType<ProfessionCard>().Select(x => x.ToTransferObject());
        var hobbiesCards = allCards.OfType<HobbiesCard>().Select(x => x.ToTransferObject());
        var ageCards = allCards.OfType<AgeCard>().Select(x => x.ToTransferObject());
        var sexCards = allCards.OfType<SexCard>().Select(x => x.ToTransferObject());
        var factCards = allCards.OfType<FactCard>().Select(x => x.ToTransferObject());

        var packEvents = packs.Select(x => x.ToTransferObject());
        var personalityEvents = personalities.Select(x => x.ToTransferObject());

        yield return new GameContentHydrated(
            packEvents,
            personalityEvents,
            professionCards,
            hobbiesCards,
            ageCards,
            sexCards,
            factCards
        );
    }
}
