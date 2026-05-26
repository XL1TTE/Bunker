using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Persistence.Contracts;

/// <summary>
/// Specialized repository interface for all cards.
/// </summary>
public interface ICardRepository : IRepository<Card, Card.Id>;

/// <summary>
/// Specialized repository interface for Age cards.
/// </summary>
public interface IAgeCardRepository : IRepository<AgeCard, Card.Id>;

/// <summary>
/// Specialized repository interface for Profession cards.
/// </summary>
public interface IProfessionCardRepository : IRepository<ProfessionCard, Card.Id>;

/// <summary>
/// Specialized repository interface for Hobbies cards.
/// </summary>
public interface IHobbiesCardRepository : IRepository<HobbiesCard, Card.Id>;

/// <summary>
/// Specialized repository interface for Fact cards.
/// </summary>
public interface IFactCardRepository : IRepository<FactCard, Card.Id>;

/// <summary>
/// Specialized repository interface for Sex cards.
/// </summary>
public interface ISexCardRepository : IRepository<SexCard, Card.Id>;
