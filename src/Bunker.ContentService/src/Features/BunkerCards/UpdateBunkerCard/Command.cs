using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.BunkerCards.UpdateBunkerCard;

public readonly record struct UpdateBunkerCard(
    BunkerCard.Id Id,
    string Catastrophe,
    string SurvivalDuration,
    string BunkerEnvironment)
{
    public abstract record Result
    {
        public record Success(BunkerCard Card) : Result;
        public record NotFound : Result;
    }

    public static Result.Success Success(BunkerCard card) => new(card);
    public static Result.NotFound NotFound() => new();
}