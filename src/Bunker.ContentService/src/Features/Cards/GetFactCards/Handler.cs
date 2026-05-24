using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Cards.GetFactCards;

[WolverineHandler]
public static class GetFactCardsHandler
{
    public static async Task<GetFactCards.Result> Handle(GetFactCards query, ICardQueries queries)
    {
        var (total, cards) = await queries.GetFactCardsAsync(skip: query.Skip, take: query.Take);
        return GetFactCards.Success(total, cards);
    }
}
