namespace Bunker.ContentService.Domain;

public record HobbiesCard : Card
{
    public string Hobbies { get; init; }

    internal HobbiesCard(Card.Id PublicId, string Hobbies) : base(PublicId)
        => (this.PublicId, this.Hobbies) = (PublicId, Hobbies);

    public static HobbiesCard CreateNew(string Hobbies) => CreateValid(Id.New(), Hobbies);
    public static HobbiesCard CreateNew(Id Id, string Hobbies) => CreateValid(Id, Hobbies);
        
    private static HobbiesCard CreateValid(Id Id, string Hobbies)
        => string.IsNullOrEmpty(Hobbies) | Hobbies.Length < 6
        ? throw new ArgumentException("Hobbies must be at least 6 characters long.")
        : new HobbiesCard(Id, Hobbies);
}

public static class HobbiesCardExtensions
{
    extension(HobbiesCard card)
    {
        public HobbiesCard WithHobbies(string Hobbies) => card with {Hobbies = Hobbies};
    }
}
