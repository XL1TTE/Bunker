using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.CardPacks.AddCardToPack;

public readonly record struct AddCardToPack(CardPack.Id CardPackId, Card.Id CardId)
{
    public abstract record Result
    {
        public record Success(CardPack Pack) : Result;
        public record NotFound : Result;
    }

    public static Result.Success Success(CardPack pack) => new(pack);
    public static Result.NotFound NotFound() => new();
}
