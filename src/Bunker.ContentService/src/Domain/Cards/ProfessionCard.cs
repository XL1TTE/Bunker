namespace Bunker.ContentService.Domain;

public record ProfessionCard : Card
{
    public string Profession { get; init; }

    internal ProfessionCard(Card.Id PublicId, string Profession) : base(PublicId)
        => (this.PublicId, this.Profession) = (PublicId, Profession);

    public static ProfessionCard CreateNew(string Profession) => CreateValid(Id.New(), Profession);
    public static ProfessionCard Restore(Id Id, string Profession) => CreateValid(Id, Profession);
        
    private static ProfessionCard CreateValid(Id Id, string Profession)
        => string.IsNullOrEmpty(Profession) | Profession.Length < 4
        ? throw new ArgumentException("Profession must be at least 4 characters long.")
        : new ProfessionCard(Id, Profession);
}

public static class ProfessionCardExtensions
{
    extension(ProfessionCard card)
    {
        public ProfessionCard WithProfession(string Profession) => card with {Profession = Profession};
    }
}
