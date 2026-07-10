using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Cards.GetHealthCards;

[WolverineHandler]
public static class GetHealthCardsHandler
{
    public static async Task<GetHealthCards.Result> Handle(GetHealthCards query, ICardQueries queries)
    {
        var (total, cards) = await queries.GetHealthCardsAsync(skip: query.Skip, take: query.Take);
        return GetHealthCards.Success(total, cards);
    }
}