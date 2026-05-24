namespace Bunker.ContentService.Transfers;

public abstract partial class Transfer
{
    public abstract record Card(Guid Id);

    /// <summary>
    /// Transfer object of profession card.
    /// </summary>
    /// <param name="Id">Id of the card.</param>
    /// <param name="Profession">Profession on card.</param>
    public record ProfessionCard(Guid Id, string Profession) : Card(Id);
 
    public record HobbiesCard(Guid Id, string Hobbies) : Card(Id);
    public record AgeCard(Guid Id, int Age) : Card(Id);
    public record SexCard(Guid Id, string Sex) : Card(Id);
    public record FactCard(Guid Id, string Fact) : Card(Id);
}


