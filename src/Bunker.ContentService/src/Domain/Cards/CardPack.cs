using System.Collections.ObjectModel;

namespace Bunker.ContentService.Domain;

/// <summary>
/// Aggregate root for card packs. Each card pack has a unique public identifier, a title, a description, and a collection of cards. The title must be at least 6 characters long, and the description must be at least 10 characters long.
/// </summary>
/// <param name="PublicId"></param>
/// <param name="Title">Title of the card pack.</param>
/// <param name="Description">Description of the card pack.</param>
public record CardPack(CardPack.Id PublicId, string Title, string Description)
{
    public readonly record struct Id(Guid Value)
    {
        public static Id Empty => new Id(Guid.Empty);
        public static Id New() => new Id(Guid.NewGuid());
        public static Id Create(Guid value) => new Id(value);
    }

    public Id PublicId { get; private set; } = PublicId;

    public string Title { get; private set; } =
        string.IsNullOrWhiteSpace(Title) | Title.Length < 6
        ? throw new ArgumentException("Title must be at least 6 characters long.")
        : Title;

    public string Description { get; private set; } =
        string.IsNullOrWhiteSpace(Description) | Description.Length < 10
        ? throw new ArgumentException("Description must be at least 10 characters long.")
        : Description;

    internal Collection<CardPackCards> _cards { get; set; } = [];
    public IReadOnlyCollection<CardPackCards> Cards => _cards.AsReadOnly();
}

public static class CardPackFactory
{
    public static CardPack Create(CardPack.Id publicId, string title, string description)
        => new CardPack(publicId, title, description);

    public static CardPack New(string title, string description)
        => new CardPack(CardPack.Id.New(), title, description);
}

public static class CardPackExtensions
{
    extension(CardPack pack)
    {
        public void AddCard(Card.Id id)
        {
            if (pack._cards.Any(x => x.CardId == id))
                throw new ArgumentException("Card with the same ID already exists in the card pack.");

            pack._cards.Add(new CardPackCards(pack.PublicId, id));
        }

        public void RemoveCard(Card.Id id)
        {
            if (!pack._cards.Any(x => x.CardId == id))
                throw new ArgumentException("Card with the specified ID does not exist in the card pack.");

            pack._cards.Remove(pack._cards.First(x => x.CardId == id));
        }
    }
}
