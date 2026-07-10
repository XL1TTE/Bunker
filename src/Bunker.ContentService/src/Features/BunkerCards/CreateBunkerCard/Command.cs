using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.BunkerCards.CreateBunkerCard;

public readonly record struct CreateBunkerCard(string Catastrophe, string SurvivalDuration, string BunkerEnvironment)
{
    public abstract record Result
    {
        public record Success(BunkerCard Card) : Result;
    }

    public static Result.Success Success(BunkerCard card) => new(card);
}