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
};
