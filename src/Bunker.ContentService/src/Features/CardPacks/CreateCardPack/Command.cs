using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.CardPacks.CreateCardPack;

public readonly record struct CreateCardPack(string Title, string Description, IEnumerable<Guid> CardIds)
{
    public abstract record Result
    {
        public record Success(CardPack Pack) : Result;
    }

    public static Result.Success Success(CardPack pack) => new(pack);
}
