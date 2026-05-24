using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.Cards.GetFactCards;

public readonly record struct GetFactCards(int Skip, int Take)
{
    public abstract record Result
    {
        public record Success(int total, IReadOnlyCollection<FactCard> Cards) : Result;
    }
    public static Result.Success Success(int total, IReadOnlyCollection<FactCard> cards) => new Result.Success(total, cards);
}
