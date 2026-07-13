namespace Bunker.ContentService.Domain;

public record LuggageCard : Card
{
    public string Luggage { get; init; }

    internal LuggageCard(Card.Id PublicId, string Luggage) : base(PublicId)
        => (this.PublicId, this.Luggage) = (PublicId, Luggage);

    public static LuggageCard CreateNew(string Luggage) => CreateValid(Id.New(), Luggage);
    public static LuggageCard Restore(Id Id, string Luggage) => CreateValid(Id, Luggage);

    private static LuggageCard CreateValid(Id Id, string Luggage)
        => string.IsNullOrWhiteSpace(Luggage) | Luggage.Length < 4
        ? throw new ArgumentException("Luggage must be at least 4 characters long.")
        : new LuggageCard(Id, Luggage);
}

public static class LuggageCardExtensions
{
    extension(LuggageCard card)
    {
        public LuggageCard WithLuggage(string Luggage) => card with {Luggage = Luggage};
    }
}