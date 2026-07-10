using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.CardPacks.UpdateCardPack;

public readonly record struct UpdateCardPack(CardPack.Id Id, string Title, string Description, string GenerationPrompt, IEnumerable<Guid> CardIds)
{
    public abstract record Result
    {
        public record Success(CardPack Pack) : Result;
        public record NotFound : Result;
    }

    public static Result.Success Success(CardPack pack) => new(pack);
    public static Result.NotFound NotFound() => new();
}
