using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.Cards.CreateHobbiesCard;

public readonly record struct CreateHobbiesCard(string Hobbies)
{
    public abstract record Result
    {
        public record Success(HobbiesCard Card) : Result;
    }
    public static Result.Success Success(HobbiesCard created) => new Result.Success(created);
}
