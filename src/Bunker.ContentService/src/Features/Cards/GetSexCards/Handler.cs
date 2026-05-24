using Bunker.ContentService.Persistence.Contracts;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Cards.GetSexCards;

[WolverineHandler]
public static class GetSexCardsHandler
{
    public static async Task<GetSexCards.Result> Handle(GetSexCards query, ICardQueries queries)
    {
        var (total, cards) = await queries.GetSexCardsAsync(skip: query.Skip, take: query.Take);
        return GetSexCards.Success(total, cards);
    }
}
