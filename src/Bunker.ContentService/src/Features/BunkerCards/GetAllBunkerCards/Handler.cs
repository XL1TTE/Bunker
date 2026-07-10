using Bunker.ContentService.Persistence.Contracts;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.BunkerCards.GetAllBunkerCards;

[WolverineHandler]
public static class GetAllBunkerCardsHandler
{
    public static async Task<GetAllBunkerCards.Result> Handle(GetAllBunkerCards query, IBunkerCardQueries queries)
    {
        var domains = await queries.GetAllAsync();
        return GetAllBunkerCards.Success(domains);
    }
}