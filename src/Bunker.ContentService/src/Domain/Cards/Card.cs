namespace Bunker.ContentService.Domain;

/// <summary>
/// Aggregate root for all cards. Each card has a unique public identifier and a type-specific payload. The payload is defined in the derived record types: ProfessionCard, HobbiesCard, AgeCard, SexCard, and FactCard.
/// </summary>
public abstract record Card(Card.Id PublicId)
{
    public readonly record struct Id(Guid Value)
    {
        public static Id Empty { get; } = new Id(Guid.Empty);
        public static Id New() => new Id(Guid.NewGuid());
        public static Id Create(Guid value) => new Id(value);
    }

    public static class Create {
        
        public static ProfessionCard ProfessionCard(string Profession) 
            => string.IsNullOrEmpty(Profession) | Profession.Length < 6 
            ?  throw new ArgumentException("Profession must be at least 6 characters long.")
            : new ProfessionCard(Id.New(), Profession);

        public static HobbiesCard HobbiesCard(string Hobbies) 
            => string.IsNullOrEmpty(Hobbies) | Hobbies.Length < 6 
            ?  throw new ArgumentException("Hobbies must be at least 6 characters long.")
            : new HobbiesCard(Id.New(), Hobbies);

        public static AgeCard AgeCard(int Age) 
            => int.IsNegative(Age) | Age >= 255 
            ?  throw new ArgumentException("Age must in [0, 255] interval.")
            : new AgeCard(Id.New(), Age);

        public static SexCard SexCard(SexCard.SexEnum Sex)
            => new SexCard(Id.New(), Sex);

        public static SexCard SexCard(byte Sex)
            => new SexCard(Id.New(), (SexCard.SexEnum)Sex);

        public static SexCard SexCard(ReadOnlySpan<char> Sex) 
            => !Enum.TryParse<SexCard.SexEnum>(Sex, true, out var sex) 
            ? throw new ArgumentException("Sex string should be either Make or Female.")
            : new SexCard(Id.New(), sex);
    }
};

public record ProfessionCard(Card.Id PublicId, string Profession) : Card(PublicId);
public record HobbiesCard(Card.Id PublicId, string Hobbies) : Card(PublicId);
public record AgeCard(Card.Id PublicId, int Age) : Card(PublicId);
public record SexCard(Card.Id PublicId, SexCard.SexEnum Sex) : Card(PublicId)
{
    public enum SexEnum: byte { Male = 0, Female = 1 }
};
public record FactCard(Card.Id PublicId, string Fact) : Card(PublicId);
