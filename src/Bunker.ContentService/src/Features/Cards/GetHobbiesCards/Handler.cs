using Bunker.ContentService.Persistence.Contracts;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Cards.GetHobbiesCards;

[WolverineHandler]
public static class GetHobbiesCardsHandler
{
    public static async Task<GetHobbiesCards.Result> Handle(GetHobbiesCards query, ICardQueries queries)
    {
        var (total, cards) = await queries.GetHobbiesCardsAsync(skip: query.Skip, take: query.Take);
        return GetHobbiesCards.Success(total, cards);
    }
}
