using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.BunkerCards.GetBunkerCard;

public readonly record struct GetBunkerCard(BunkerCard.Id Id)
{
    public abstract record Result
    {
        public record Success(BunkerCard Card) : Result;
        public record NotFound : Result;
    }

    public static Result.Success Success(BunkerCard card) => new(card);
    public static Result.NotFound NotFound() => new();
}