using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Features.CardPacks.GetAllCardPacks;

public readonly record struct GetAllCardPacks()
{
    public abstract record Result
    {
        public record Success(IReadOnlyCollection<CardPack> Packs) : Result;
    }

    public static Result.Success Success(IReadOnlyCollection<CardPack> packs) => new(packs);
}
