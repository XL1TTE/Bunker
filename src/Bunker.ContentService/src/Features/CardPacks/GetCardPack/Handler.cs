using Bunker.ContentService.Persistence.Contracts;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.CardPacks.GetCardPack;

[WolverineHandler]
public static class GetCardPackHandler
{
    public static async Task<GetCardPack.Result> Handle(GetCardPack query, ICardPackQueries queries)
    {
        var domain = await queries.TryFindAsync(query.Id);
        return domain is null ? GetCardPack.NotFound() : GetCardPack.Success(domain);
    }
}
