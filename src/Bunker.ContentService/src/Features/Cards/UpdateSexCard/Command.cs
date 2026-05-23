using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.Cards.UpdateSexCard;

public readonly record struct UpdateSexCard(Card.Id Id, string Sex)
{
    public abstract record Result
    {
        public record Success(SexCard Card) : Result;
        public record NotFound() : Result;
    }
    public static Result.Success Success(SexCard updated) => new Result.Success(updated);
    public static Result.NotFound NotFound() => new Result.NotFound();
}
