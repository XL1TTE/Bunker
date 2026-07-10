using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.Cards.UpdateLuggageCard;

public readonly record struct UpdateLuggageCard(Card.Id Id, string Luggage)
{
    public abstract record Result
    {
        public record Success(LuggageCard Card) : Result;
        public record NotFound() : Result;
    }
    public static Result.Success Success(LuggageCard updated) => new Result.Success(updated);
    public static Result.NotFound NotFound() => new Result.NotFound();
}