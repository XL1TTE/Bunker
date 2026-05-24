using Wolverine.Attributes;
using Bunker.ContentService.Persistence.Contracts;

namespace Bunker.ContentService.Features.Cards.GetProfessionCards;

[WolverineHandler]
public static class GetProfessionCardsHandler
{
    public static async Task<GetProfessionCards.Result> Handle(GetProfessionCards query, ICardQueries queries)
    {
        var (total, cards) = await queries.GetProfessionCardsAsync(skip: query.Skip, take: query.Take);
        return GetProfessionCards.Success(total, cards);
    }
}
