using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.Cards.GetAgeCards;

public readonly record struct GetAgeCards(int Skip, int Take)
{
    public abstract record Result
    {
        public record Success(int total, IReadOnlyCollection<AgeCard> Cards) : Result;
    }
    public static Result.Success Success(int total, IReadOnlyCollection<AgeCard> cards) => new Result.Success(total, cards);
}
