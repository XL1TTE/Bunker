using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.Cards.GetHobbiesCards;

public readonly record struct GetHobbiesCards(int Skip, int Take)
{
    public abstract record Result
    {
        public record Success(int total, IReadOnlyCollection<HobbiesCard> Cards) : Result;
    }
    public static Result.Success Success(int total, IReadOnlyCollection<HobbiesCard> cards) => new Result.Success(total, cards);
}
