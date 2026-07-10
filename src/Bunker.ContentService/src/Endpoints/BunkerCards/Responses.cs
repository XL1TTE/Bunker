using Bunker.ContentService.Transfers;

namespace Bunker.ContentService.Api.BunkerCards.Endpoints.Responses;

/// <summary>
/// Container for bunker card related HTTP response models.
/// </summary>
public abstract record BunkerCardResponse
{
    /// <summary>
    /// Response indicating that a bunker card was successfully created.
    /// </summary>
    /// <param name="Card">The created bunker card data.</param>
    public readonly record struct Created(Transfer.BunkerCard Card);

    /// <summary>
    /// Response indicating that a bunker card was successfully updated.
    /// </summary>
    /// <param name="Card">The updated bunker card data.</param>
    public readonly record struct Updated(Transfer.BunkerCard Card);

    /// <summary>
    /// Response containing a single bunker card.
    /// </summary>
    /// <param name="Card">The bunker card data.</param>
    public readonly record struct Single(Transfer.BunkerCard Card);

    /// <summary>
    /// Response containing a collection of all bunker cards.
    /// </summary>
    /// <param name="Cards">The collection of bunker cards.</param>
    public readonly record struct All(IEnumerable<Transfer.BunkerCard> Cards);
}