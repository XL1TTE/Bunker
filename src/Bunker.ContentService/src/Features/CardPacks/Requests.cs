namespace Bunker.ContentService.Features.CardPacks;

/// <summary>Request to create a new Card Pack.</summary>
public record CreateCardPack(string Title, string Description, IEnumerable<Guid> CardIds);
/// <summary>Request to update an existing Card Pack.</summary>
public record UpdateCardPack(Guid Id, string Title, string Description, IEnumerable<Guid> CardIds);
/// <summary>Request to delete a Card Pack.</summary>
public record DeleteCardPack(Guid Id);
/// <summary>Request to retrieve a specific Card Pack by its ID.</summary>
public record GetCardPack(Guid Id);
/// <summary>Request to retrieve all Card Packs.</summary>
public record GetAllCardPacks();

/// <summary>Request to add a specific Card to a Card Pack.</summary>
public record AddCardToPack(Guid CardPackId, Guid CardId);
/// <summary>Request to remove a specific Card from a Card Pack.</summary>
public record RemoveCardFromPack(Guid CardPackId, Guid CardId);
