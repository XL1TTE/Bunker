using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.BunkerCards.GetAllBunkerCards;

public readonly record struct GetAllBunkerCards()
{
    public abstract record Result
    {
        public record Success(IReadOnlyCollection<BunkerCard> Cards) : Result;
    }

    public static Result.Success Success(IReadOnlyCollection<BunkerCard> cards) => new(cards);
}