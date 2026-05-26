using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.CardPacks.RemoveCardFromPack;

public readonly record struct RemoveCardFromPack(CardPack.Id CardPackId, Card.Id CardId)
{
    public abstract record Result
    {
        public record Success(CardPack Pack) : Result;
        public record NotFound : Result;
    }

    public static Result.Success Success(CardPack pack) => new(pack);
    public static Result.NotFound NotFound() => new();
}
