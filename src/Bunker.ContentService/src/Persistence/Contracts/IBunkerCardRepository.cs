using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Persistence.Contracts;

public interface IBunkerCardRepository : IRepository<BunkerCard, BunkerCard.Id>;