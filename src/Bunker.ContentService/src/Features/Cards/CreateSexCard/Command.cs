using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.Cards.CreateSexCard;

public readonly record struct CreateSexCard(string Sex)
{
    public abstract record Result
    {
        public record Success(SexCard Card) : Result;
    }
    public static Result.Success Success(SexCard created) => new Result.Success(created);
}
