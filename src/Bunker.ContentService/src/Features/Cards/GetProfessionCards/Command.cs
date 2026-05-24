using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.Cards.GetProfessionCards;

public readonly record struct GetProfessionCards(int Take, int Skip)
{
    public abstract record Result
    {
        public record Success(int Total, IReadOnlyCollection<ProfessionCard> Cards) : Result;
    }
    public static Result.Success Success(int total, IReadOnlyCollection<ProfessionCard> created) => new Result.Success(total, created);
}
