using Bunker.ContentService.Persistence.Contracts;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.BunkerCards.GetBunkerCard;

[WolverineHandler]
public static class GetBunkerCardHandler
{
    public static async Task<GetBunkerCard.Result> Handle(GetBunkerCard query, IBunkerCardQueries queries)
    {
        var domain = await queries.TryFindAsync(query.Id);
        return domain is null ? GetBunkerCard.NotFound() : GetBunkerCard.Success(domain);
    }
}