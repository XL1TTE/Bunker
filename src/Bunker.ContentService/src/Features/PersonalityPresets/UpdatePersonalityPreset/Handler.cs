using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.PersonalityPresets.UpdatePersonalityPreset;

[WolverineHandler]
public static class UpdatePersonalityPresetHandler
{
    public static async Task<UpdatePersonalityPreset.Result> Handle(UpdatePersonalityPreset command, IUnitOfWork uow)
    {
        var repository = uow.GetRepository<IPersonalityPresetRepository>();
        var domain = await repository.TryFindAsync(command.Id);
        if (domain is null) return UpdatePersonalityPreset.NotFound();
            
        domain.UpdateTitle(command.Title);
        domain.UpdateDescription(command.Description);
        
        repository.Update(domain);
        await uow.SaveChangesAsync();
        
        return UpdatePersonalityPreset.Success(domain);
    }
}
