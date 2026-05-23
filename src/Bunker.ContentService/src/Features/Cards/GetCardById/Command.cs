using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.Cards.GetCardById;

public readonly record struct GetCardById(Card.Id Id)
{
    public abstract record Result
    {
        public record Success(Card Card) : Result;
        public record NotFound() : Result;
    }
    public static Result.Success Success(Card card) => new Result.Success(card);
    public static Result.NotFound NotFound() => new Result.NotFound();
}