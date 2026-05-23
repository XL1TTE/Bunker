using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.Cards.UpdateAgeCard;

public readonly record struct UpdateAgeCard(Card.Id Id, int Age)
{
    public abstract record Result
    {
        public record Success(AgeCard Card) : Result;
        public record NotFound() : Result;
    }
    public static Result.Success Success(AgeCard updated) => new Result.Success(updated);
    public static Result.NotFound NotFound() => new Result.NotFound();
}
