using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.Cards.GetSexCards;

public readonly record struct GetSexCards(int Skip, int Take)
{
    public abstract record Result
    {
        public record Success(int total, IReadOnlyCollection<SexCard> Cards) : Result;
    }
    public static Result.Success Success(int total, IReadOnlyCollection<SexCard> cards) => new Result.Success(total, cards);
}
