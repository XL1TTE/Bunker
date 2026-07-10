namespace Bunker.ContentService.Transfers;

public abstract partial class Transfer
{
    public readonly record struct CardPack(Guid Id, string Title, string Description, string GenerationPrompt, IReadOnlyCollection<Guid> CardIds);

    public readonly record struct CardPackPreview(Guid Id, string Title, string Description);
}