using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.Cards.GetFactCards;

public readonly record struct GetFactCards()
{
    public abstract record Result
    {
        public record Success(IReadOnlyCollection<FactCard> Cards) : Result;
    }
    public static Result.Success Success(IReadOnlyCollection<FactCard> cards) => new Result.Success(cards);
}