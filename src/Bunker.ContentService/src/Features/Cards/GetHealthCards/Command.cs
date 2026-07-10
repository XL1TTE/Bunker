using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.Cards.GetHealthCards;

public readonly record struct GetHealthCards(int Skip, int Take)
{
    public abstract record Result
    {
        public record Success(int Total, IReadOnlyCollection<HealthCard> Cards) : Result;
    }
    public static Result.Success Success(int total, IReadOnlyCollection<HealthCard> cards) => new Result.Success(total, cards);
}