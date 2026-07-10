using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.Cards.CreateHealthCard;

public readonly record struct CreateHealthCard(string Health)
{
    public abstract record Result
    {
        public record Success(HealthCard Card) : Result;
    }
    public static Result.Success Success(HealthCard created) => new Result.Success(created);
}