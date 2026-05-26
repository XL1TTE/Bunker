using Bunker.ContentService.Persistence.Contracts;
using Bunker.ContentService.Persistence.Entities;
using Bunker.ContentService.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Bunker.ContentService.Persistence;

public partial class ContentDbContext : IPersonalityPresetRepository
{
    async Task<Domain.PersonalityPreset?> IRepository<Domain.PersonalityPreset, Domain.PersonalityPreset.Id>.TryFindAsync(Domain.PersonalityPreset.Id key)
        => (await PersonalityPresets.FirstOrDefaultAsync(x => x.PublicId == key.Value))?.ToDomain();

    void IRepository<Domain.PersonalityPreset, Domain.PersonalityPreset.Id>.Add(Domain.PersonalityPreset aggregate)
        => PersonalityPresets.Add(aggregate.ToEntity());

    void IRepository<Domain.PersonalityPreset, Domain.PersonalityPreset.Id>.Delete(Domain.PersonalityPreset aggregate)
    {
        var entity = PersonalityPresets.FirstOrDefault(x => x.PublicId == aggregate.PublicId.Value);
        if (entity != null)
        {
            PersonalityPresets.Remove(entity);
        }
    }

    bool IRepository<Domain.PersonalityPreset, Domain.PersonalityPreset.Id>.Update(Domain.PersonalityPreset aggregate)
    {
        var origin = PersonalityPresets.FirstOrDefault(x => x.PublicId == aggregate.PublicId.Value);
        if (origin is null) return false;

        origin.ApplyUpdate(aggregate.ToEntity());
        return true;
    }
}
