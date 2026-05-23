using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.Cards.GetHobbiesCards;

public readonly record struct GetHobbiesCards()
{
    public abstract record Result
    {
        public record Success(IReadOnlyCollection<HobbiesCard> Cards) : Result;
    }
    public static Result.Success Success(IReadOnlyCollection<HobbiesCard> cards) => new Result.Success(cards);
}