namespace Bunker.ContentService.Domain;

public record AgeCard : Card
{
    public int Age { get; init; }

    internal AgeCard(Card.Id PublicId, int Age) : base(PublicId)
        => (this.PublicId, this.Age) = (PublicId, Age);

    public static AgeCard CreateNew(int Age) => CreateValid(Id.New(), Age);
    public static AgeCard Restore(Card.Id Id, int Age) => CreateValid(Id, Age);
        
    private static AgeCard CreateValid(Card.Id Id, int Age)
        => int.IsNegative(Age) | Age >= 255
        ? throw new ArgumentException("Age must in [0, 255] interval.")
        : new AgeCard(Id, Age);
}

public static class AgeCardExtensions
{
    extension(AgeCard card)
    {
        public AgeCard WithAge(int Age) => card with {Age = Age};
    }
}
