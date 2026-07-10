using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.Cards.CreateLuggageCard;

public readonly record struct CreateLuggageCard(string Luggage)
{
    public abstract record Result
    {
        public record Success(LuggageCard Card) : Result;
    }
    public static Result.Success Success(LuggageCard created) => new Result.Success(created);
}