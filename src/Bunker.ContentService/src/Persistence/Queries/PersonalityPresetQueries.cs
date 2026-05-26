using Bunker.ContentService.Persistence.Contracts;
using Bunker.ContentService.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Bunker.ContentService.Persistence.Queries;

public class DbContextPersonalityPresetQueries(ContentDbContext dbContext) : IPersonalityPresetQueries
{
    public async Task<Domain.PersonalityPreset?> TryFindAsync(Domain.PersonalityPreset.Id id)
        => (await dbContext.PersonalityPresets.AsNoTracking()
            .FirstOrDefaultAsync(x => x.PublicId == id.Value))?.ToDomain();

    public async Task<IReadOnlyCollection<Domain.PersonalityPreset>> GetAllAsync()
    {
        var entities = await dbContext.PersonalityPresets.AsNoTracking()
            .ToListAsync();
        return entities.Select(x => x.ToDomain()).ToList();
    }
}
