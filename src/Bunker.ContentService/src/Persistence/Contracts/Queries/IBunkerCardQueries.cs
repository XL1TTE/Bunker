using Bunker.ContentService.Domain;

namespace Bunker.ContentService.Persistence.Contracts;

public interface IBunkerCardQueries
{
    Task<BunkerCard?> TryFindAsync(BunkerCard.Id id);
    Task<IReadOnlyCollection<BunkerCard>> GetAllAsync();
}