using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.Cards.CreateFactCard;

public readonly record struct CreateFactCard(string Fact)
{
    public abstract record Result
    {
        public record Success(FactCard Card) : Result;
    }
    public static Result.Success Success(FactCard created) => new Result.Success(created);
}
