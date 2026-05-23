using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.Cards.GetSexCards;

public readonly record struct GetSexCards()
{
    public abstract record Result
    {
        public record Success(IReadOnlyCollection<SexCard> Cards) : Result;
    }
    public static Result.Success Success(IReadOnlyCollection<SexCard> cards) => new Result.Success(cards);
}