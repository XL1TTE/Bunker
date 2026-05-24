namespace Bunker.ContentService.Domain;

public record FactCard : Card
{
    public string Fact { get; init; }

    internal FactCard(Card.Id PublicId, string Fact) : base(PublicId)
        => (this.PublicId, this.Fact) = (PublicId, Fact);

    public static FactCard CreateNew(string Fact) => CreateValid(Id.New(), Fact);
    public static FactCard Restore(Id Id, string Fact) => CreateValid(Id, Fact);
        
    private static FactCard CreateValid(Id Id, string Fact)
        => string.IsNullOrWhiteSpace(Fact) | Fact.Length >= 8
        ? throw new ArgumentException("Fact must be at least 6 characters long.")
        : new FactCard(Id, Fact);
}

public static class FactCardExtensions
{
    extension(FactCard card)
    {
        public FactCard WithFact(string Fact) => card with {Fact = Fact};
    }
}
