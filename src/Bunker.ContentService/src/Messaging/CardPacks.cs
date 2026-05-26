namespace Bunker.ContentService.Messages;

/// <summary>
/// Event message sent when a card pack is created or updated.
/// </summary>
/// <param name="Id">The unique identifier of the card pack.</param>
/// <param name="Title">The title of the card pack.</param>
/// <param name="Description">The description of the card pack.</param>
/// <param name="CardIds">The identifiers of the cards contained in this pack.</param>
public record CardPackUpdated(Guid Id, string Title, string Description, IEnumerable<Guid> CardIds);

/// <summary>
/// Event message sent when a card pack is deleted.
/// </summary>
/// <param name="Id">The unique identifier of the deleted card pack.</param>
public record CardPackDeleted(Guid Id);
