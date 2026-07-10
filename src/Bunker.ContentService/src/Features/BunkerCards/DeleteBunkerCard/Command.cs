using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.BunkerCards.DeleteBunkerCard;

public readonly record struct DeleteBunkerCard(BunkerCard.Id Id)
{
    public abstract record Result
    {
        public record Success : Result;
        public record NotFound : Result;
    }

    public static Result.Success Success() => new();
    public static Result.NotFound NotFound() => new();
}