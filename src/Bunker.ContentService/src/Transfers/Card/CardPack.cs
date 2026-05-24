namespace Bunker.ContentService.Transfers;

public abstract partial class Transfer
{
    public readonly record struct CardPack(string Id, string Title, string Description, IReadOnlyCollection<Guid> CardIds);
}

