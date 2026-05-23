using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.Cards.GetProfessionCards;

public readonly record struct GetProfessionCards()
{
    public abstract record Result
    {
        public record Success(IReadOnlyCollection<ProfessionCard> Cards) : Result;
    }
    public static Result.Success Success(IReadOnlyCollection<ProfessionCard> created) => new Result.Success(created);
}
