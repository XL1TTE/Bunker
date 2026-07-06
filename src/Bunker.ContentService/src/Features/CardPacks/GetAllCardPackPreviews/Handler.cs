using Bunker.ContentService.Persistence.Contracts;
using Bunker.ContentService.Transfers;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.CardPacks.GetAllCardPackPreviews;

[WolverineHandler]
public static class GetAllCardPackPreviewsHandler
{
    public static async Task<GetAllCardPackPreviews.Result> Handle(GetAllCardPackPreviews query, ICardPackQueries queries)
    {
        var packs = await queries.GetAllPreviewsAsync();
        var previews = packs.Select(x => x.ToPreviewObject()).ToList();
        return GetAllCardPackPreviews.Success(previews);
    }
}