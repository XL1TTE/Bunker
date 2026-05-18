
namespace Bunker.Persistence.Entities;

public class CardPack
{
    public required Guid PublicId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    
    public ICollection<CardPackCards> Cards { get; set; } = [];
}

public class CardPackCards
{
    public required Guid CardPackId { get; set; }
    public required Guid CardId { get; set; }
}

public abstract class Card
{
    public required Guid PublicId { get; set; }
}

public class ProfessionCard : Card
{
    public required string Profession { get; set; }
}
public class HobbiesCard : Card
{
    public required string Hobbies { get; set; }
}
public class AgeCard : Card
{
    public required int Age { get; set; }
}
public class SexCard : Card
{
    public enum SexEnum : byte { Male = 0, Female = 1 }
    public required SexEnum Sex { get; set; }
};
public class FactCard : Card
{
    public required string Fact { get; set; }
};
