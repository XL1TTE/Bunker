using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.Cards.GetLuggageCards;

public readonly record struct GetLuggageCards(int Skip, int Take)
{
    public abstract record Result
    {
        public record Success(int Total, IReadOnlyCollection<LuggageCard> Cards) : Result;
    }
    public static Result.Success Success(int total, IReadOnlyCollection<LuggageCard> cards) => new Result.Success(total, cards);
}