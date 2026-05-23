using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence;
using Bunker.ContentService.Persistence.Entities;
using Bunker.ContentService.Messages;
using Microsoft.EntityFrameworkCore;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.PersonalityPresets;

[WolverineHandler]
public static class PersonalityPresetHandlers
{
    public static async Task<PersonalityPresetResult> Handle(CreatePersonalityPreset command, ContentDbContext db)
    {
        var domain = PersonalityPresetFactory.New(command.Title, command.Description);
        var entity = domain.ToEntity();
        
        db.PersonalityPresets.Add(entity);
        await db.SaveChangesAsync();
        
        return PersonalityPresetResult.UpdatedSuccess(new PersonalityPresetUpdated(entity.PublicId, entity.Title, entity.Description));
    }

    public static async Task<PersonalityPresetResult> Handle(UpdatePersonalityPreset command, ContentDbContext db)
    {
        var entity = await db.PersonalityPresets.FirstOrDefaultAsync(x => x.PublicId == command.Id);
        if (entity is null) return PersonalityPresetResult.NotFound();
            
        var domain = entity.ToDomain();
        domain.UpdateTitle(command.Title);
        domain.UpdateDescription(command.Description);
        
        var updatedEntity = domain.ToEntity();
        entity.ApplyUpdate(updatedEntity);
        await db.SaveChangesAsync();
        
        return PersonalityPresetResult.UpdatedSuccess(new PersonalityPresetUpdated(entity.PublicId, entity.Title, entity.Description));
    }

    public static async Task<PersonalityPresetResult> Handle(DeletePersonalityPreset command, ContentDbContext db)
    {
        var entity = await db.PersonalityPresets.FirstOrDefaultAsync(x => x.PublicId == command.Id);
        if (entity is null) return PersonalityPresetResult.NotFound();
            
        db.PersonalityPresets.Remove(entity);
        await db.SaveChangesAsync();
        
        return PersonalityPresetResult.DeletedSuccess();
    }

    public static async Task<PersonalityPresetResult> Handle(GetPersonalityPreset query, ContentDbContext db)
    {
        var entity = await db.PersonalityPresets
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PublicId == query.Id);
            
        return entity is null ? PersonalityPresetResult.NotFound() : PersonalityPresetResult.Success(entity.ToDomain());
    }

    public static async Task<PersonalityPresetResult> Handle(GetAllPersonalityPresets query, ContentDbContext db)
    {
        var entities = await db.PersonalityPresets
            .AsNoTracking()
            .ToListAsync();
            
        return PersonalityPresetResult.Success(entities.Select(x => x.ToDomain()));
    }
}
