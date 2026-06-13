using Wolverine.Attributes;
using Bunker.ContentService.Persistence.Contracts;

namespace Bunker.ContentService.Features.Cards.GetAgeCards;

[WolverineHandler]
public static class GetAgeCardsHandler
{
    public static async Task<GetAgeCards.Result> Handle(GetAgeCards query, ICardQueries queries)
    {
        var (total, cards) = await queries.GetAgeCardsAsync(skip: query.Skip, take: query.Take);
        return GetAgeCards.Success(total, cards);
    }
}
