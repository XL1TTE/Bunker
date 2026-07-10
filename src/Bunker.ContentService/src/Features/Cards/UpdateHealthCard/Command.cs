using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.Cards.UpdateHealthCard;

public readonly record struct UpdateHealthCard(Card.Id Id, string Health)
{
    public abstract record Result
    {
        public record Success(HealthCard Card) : Result;
        public record NotFound() : Result;
    }
    public static Result.Success Success(HealthCard updated) => new Result.Success(updated);
    public static Result.NotFound NotFound() => new Result.NotFound();
}