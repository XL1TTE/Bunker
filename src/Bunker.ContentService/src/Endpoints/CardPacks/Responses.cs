using Bunker.ContentService.Transfers;

namespace Bunker.ContentService.Api.CardPacks.Endpoints.Responses;

/// <summary>
/// Container for card pack related HTTP response models.
/// </summary>
public abstract record CardPackResponse
{
    /// <summary>
    /// Response indicating that a card pack was successfully created.
    /// </summary>
    /// <param name="Pack">The created card pack data.</param>
    public readonly record struct Created(Transfer.CardPack Pack);

    /// <summary>
    /// Response indicating that a card pack was successfully updated.
    /// </summary>
    /// <param name="Pack">The updated card pack data.</param>
    public readonly record struct Updated(Transfer.CardPack Pack);

    /// <summary>
    /// Response containing a single card pack.
    /// </summary>
    /// <param name="Pack">The card pack data.</param>
    public readonly record struct Single(Transfer.CardPack Pack);

    /// <summary>
    /// Response containing a collection of all card packs.
    /// </summary>
    /// <param name="Packs">The collection of card packs.</param>
    public readonly record struct All(IEnumerable<Transfer.CardPack> Packs);
}
