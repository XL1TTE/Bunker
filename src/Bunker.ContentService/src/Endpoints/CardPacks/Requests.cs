namespace Bunker.ContentService.Api.CardPacks.Endpoints.Requests;

/// <summary>
/// Container for card pack related HTTP request models.
/// </summary>
public abstract record CardPackRequest
{
    /// <summary>
    /// Request models for POST operations.
    /// </summary>
    public abstract record Post
    {
        /// <summary>
        /// Request to create a new card pack.
        /// </summary>
        /// <param name="Title">The title of the card pack. Minimum 6 characters.</param>
        /// <param name="Description">The description of the card pack. Minimum 10 characters.</param>
        /// <param name="CardIds">A collection of unique identifiers for the cards to be included in the pack.</param>
        public readonly record struct Create(string Title, string Description, IEnumerable<Guid> CardIds);

        /// <summary>
        /// Request to add a card to an existing card pack.
        /// </summary>
        /// <param name="CardId">The unique identifier of the card to add.</param>
        public readonly record struct AddCard(Guid CardId);
    }

    /// <summary>
    /// Request models for PUT operations.
    /// </summary>
    public abstract record Put
    {
        /// <summary>
        /// Request to update an existing card pack.
        /// </summary>
        /// <param name="Title">The new title of the card pack. Minimum 6 characters.</param>
        /// <param name="Description">The new description of the card pack. Minimum 10 characters.</param>
        /// <param name="CardIds">The new set of identifiers for the cards in the pack.</param>
        public readonly record struct Update(string Title, string Description, IEnumerable<Guid> CardIds);
    }
}
