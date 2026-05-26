using Bunker.ContentService.Persistence.Contracts;
using Bunker.ContentService.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Bunker.ContentService.Persistence.Queries;

public class DbContextCardPackQueries(ContentDbContext dbContext) : ICardPackQueries
{
    public async Task<Domain.CardPack?> TryFindAsync(Domain.CardPack.Id id)
        => (await dbContext.CardPacks.AsNoTracking()
            .Include(x => x.Cards)
            .FirstOrDefaultAsync(x => x.PublicId == id.Value))?.ToDomain();

    public async Task<IReadOnlyCollection<Domain.CardPack>> GetAllAsync()
    {
        var entities = await dbContext.CardPacks.AsNoTracking()
            .Include(x => x.Cards)
            .ToListAsync();
        return entities.Select(x => x.ToDomain()).ToList();
    }
}
