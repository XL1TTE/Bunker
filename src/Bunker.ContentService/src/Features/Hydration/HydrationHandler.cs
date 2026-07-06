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
    public static async Task Handle(
        RequestGameContentHydration command,
        IMessageContext messaging,
        IHydrationQueries queries)
    {
        var packIds = command.CardPackIds.Distinct().ToList();
        var personalityIds = command.PersonalityPresetIds.Distinct().ToList();

        var (packs, personalities, allCards) = await queries.GetHydrationDataAsync(packIds, personalityIds);

        var missingPacks = packIds.Except(packs.Select(x => x.PublicId.Value)).ToList();
        var missingPersonalities = personalityIds.Except(personalities.Select(x => x.PublicId.Value)).ToList();

        if (missingPacks.Count > 0 || missingPersonalities.Count > 0)
        {
            var missingIds = missingPacks.Concat(missingPersonalities).ToList();
            await messaging.PublishAsync(new GameContentHydrationFailed(command.StartRequestId, "Some requested content IDs are missing.", missingIds));
            return;
        }

        await messaging.PublishAsync(new GameContentHydrated(
            command.StartRequestId,
            packs.Select(x => x.ToTransferObject()).ToList(),
            personalities.Select(x => x.ToTransferObject()).ToList(),
            allCards.OfType<ProfessionCard>().Select(x => x.ToTransferObject()).ToList(),
            allCards.OfType<HobbiesCard>().Select(x => x.ToTransferObject()).ToList(),
            allCards.OfType<AgeCard>().Select(x => x.ToTransferObject()).ToList(),
            allCards.OfType<SexCard>().Select(x => x.ToTransferObject()).ToList(),
            allCards.OfType<FactCard>().Select(x => x.ToTransferObject()).ToList()
        ));
    }
}