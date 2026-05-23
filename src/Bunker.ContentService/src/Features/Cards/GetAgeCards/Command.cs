using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.Cards.GetAgeCards;

public readonly record struct GetAgeCards()
{
    public abstract record Result
    {
        public record Success(IReadOnlyCollection<AgeCard> Cards) : Result;
    }
    public static Result.Success Success(IReadOnlyCollection<AgeCard> cards) => new Result.Success(cards);
}