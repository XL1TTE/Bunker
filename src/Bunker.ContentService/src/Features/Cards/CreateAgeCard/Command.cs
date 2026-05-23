using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.Cards.CreateAgeCard;

public readonly record struct CreateAgeCard(int Age)
{
    public abstract record Result
    {
        public record Success(AgeCard Card) : Result;
    }
    public static Result.Success Success(AgeCard created) => new Result.Success(created);
}
