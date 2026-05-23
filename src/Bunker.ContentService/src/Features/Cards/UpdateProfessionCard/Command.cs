using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.Cards.UpdateProfessionCard;

public readonly record struct UpdateProfessionCard(Card.Id Id, string Profession)
{
    public abstract record Result
    {
        public record Success(ProfessionCard Card) : Result;
        public record NotFound() : Result;
    }
    public static Result.Success Success(ProfessionCard updated) => new Result.Success(updated);
    public static Result.NotFound NotFound() => new Result.NotFound();
}
