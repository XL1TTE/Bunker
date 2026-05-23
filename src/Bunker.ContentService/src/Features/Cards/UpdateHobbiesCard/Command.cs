using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.Cards.UpdateHobbiesCard;

public readonly record struct UpdateHobbiesCard(Card.Id Id, string Hobbies)
{
    public abstract record Result
    {
        public record Success(HobbiesCard Card) : Result;
        public record NotFound() : Result;
    }
    public static Result.Success Success(HobbiesCard updated) => new Result.Success(updated);
    public static Result.NotFound NotFound() => new Result.NotFound();
}
