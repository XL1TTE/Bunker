using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.Cards.DeleteCard;

public readonly record struct DeleteCard(Card.Id Id)
{
    public abstract record Result
    {
        public record Success() : Result;
        public record NotFound() : Result;
    }
    public static Result.Success Success() => new Result.Success();
    public static Result.NotFound NotFound() => new Result.NotFound();
}