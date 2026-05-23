using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.Cards.CreateProfessionCard;

public readonly record struct CreateProfessionCard(string Profession)
{
    public abstract record Result
    {
        public record Success(ProfessionCard Card) : Result;
    }
    public static Result.Success Success(ProfessionCard created) => new Result.Success(created);
}
