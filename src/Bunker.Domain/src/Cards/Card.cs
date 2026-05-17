namespace Bunker.Domain.Cards;

public abstract record Card
{
    public readonly record struct CardId(Guid Value)
    {
        public static CardId Empty { get; } = new CardId(Guid.Empty);
        public static CardId New => new CardId(Guid.NewGuid());
        public static CardId Restore(Guid value) => new CardId(value);
    }

    public static class Create {
        
        public static ProfessionCard ProfessionCard(string Profession) 
            => string.IsNullOrEmpty(Profession) | Profession.Length < 6 
            ?  throw new ArgumentException("Profession must be at least 6 characters long.")
            : new ProfessionCard(Profession);
            
        public static HobbiesCard HobbiesCard(string Hobbies) 
            => string.IsNullOrEmpty(Hobbies) | Hobbies.Length < 6 
            ?  throw new ArgumentException("Hobbies must be at least 6 characters long.")
            : new HobbiesCard(Hobbies);
            
        public static AgeCard AgeCard(int Age) 
            => int.IsNegative(Age) | Age >= 255 
            ?  throw new ArgumentException("Age must in [0, 255] interval.")
            : new AgeCard(Age);
            
        public static SexCard SexCard(SexCard.SexEnum Sex) 
            => new SexCard(Sex);
            
        public static SexCard SexCard(byte Sex) 
            => new SexCard((SexCard.SexEnum)Sex);
            
        public static SexCard SexCard(ReadOnlySpan<char> Sex) 
            => !Enum.TryParse<SexCard.SexEnum>(Sex, true, out var sex) 
            ? throw new ArgumentException("Sex string should be either Make or Female.")
            : new SexCard(sex);
    }
};

public record ProfessionCard(string Profession) : Card;
public record HobbiesCard(string Hobbies) : Card;
public record AgeCard(int Age) : Card;
public record SexCard(SexCard.SexEnum Sex) : Card
{
    public enum SexEnum: byte { Male = 0, Female = 1 }
};
public record FactCard(string Fact) : Card;
