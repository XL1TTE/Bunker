using Bunker.ContentService.Persistence.Contracts;
using Bunker.ContentService.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Bunker.ContentService.Persistence.Queries;

public class DbContextBunkerCardQueries(ContentDbContext dbContext) : IBunkerCardQueries
{
    public async Task<Domain.BunkerCard?> TryFindAsync(Domain.BunkerCard.Id id)
        => (await dbContext.BunkerCards.AsNoTracking()
            .FirstOrDefaultAsync(x => x.PublicId == id.Value))?.ToDomain();

    public async Task<IReadOnlyCollection<Domain.BunkerCard>> GetAllAsync()
    {
        var entities = await dbContext.BunkerCards.AsNoTracking()
            .ToListAsync();
        return entities.Select(x => x.ToDomain()).ToList();
    }
}