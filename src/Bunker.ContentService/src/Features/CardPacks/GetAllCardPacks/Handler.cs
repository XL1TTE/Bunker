using Bunker.ContentService.Persistence.Contracts;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.CardPacks.GetAllCardPacks;

[WolverineHandler]
public static class GetAllCardPacksHandler
{
    public static async Task<GetAllCardPacks.Result> Handle(GetAllCardPacks query, ICardPackQueries queries)
    {
        var domains = await queries.GetAllAsync();
        return GetAllCardPacks.Success(domains);
    }
}
