using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.Cards.UpdateFactCard;

public readonly record struct UpdateFactCard(Card.Id Id, string Fact)
{
    public abstract record Result
    {
        public record Success(FactCard Card) : Result;
        public record NotFound() : Result;
    }
    public static Result.Success Success(FactCard updated) => new Result.Success(updated);
    public static Result.NotFound NotFound() => new Result.NotFound();
}
