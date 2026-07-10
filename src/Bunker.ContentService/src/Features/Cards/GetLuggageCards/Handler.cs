using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Cards.GetLuggageCards;

[WolverineHandler]
public static class GetLuggageCardsHandler
{
    public static async Task<GetLuggageCards.Result> Handle(GetLuggageCards query, ICardQueries queries)
    {
        var (total, cards) = await queries.GetLuggageCardsAsync(skip: query.Skip, take: query.Take);
        return GetLuggageCards.Success(total, cards);
    }
}