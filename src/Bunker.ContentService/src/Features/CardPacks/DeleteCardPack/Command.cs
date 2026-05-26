using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.CardPacks.DeleteCardPack;

public readonly record struct DeleteCardPack(CardPack.Id Id)
{
    public abstract record Result
    {
        public record Success : Result;
        public record NotFound : Result;
    }

    public static Result.Success Success() => new();
    public static Result.NotFound NotFound() => new();
}
