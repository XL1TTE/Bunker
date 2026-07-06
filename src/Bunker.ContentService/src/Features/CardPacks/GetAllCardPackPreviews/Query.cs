using Bunker.ContentService.Transfers;

namespace Bunker.ContentService.Features.CardPacks.GetAllCardPackPreviews;

public readonly record struct GetAllCardPackPreviews()
{
    public abstract record Result
    {
        public record Success(IReadOnlyCollection<Transfer.CardPackPreview> Previews) : Result;
    }

    public static Result.Success Success(IReadOnlyCollection<Transfer.CardPackPreview> previews) => new(previews);
}