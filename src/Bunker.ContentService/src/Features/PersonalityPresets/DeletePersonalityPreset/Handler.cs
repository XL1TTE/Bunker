using Bunker.ContentService.Persistence.Contracts;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.PersonalityPresets.DeletePersonalityPreset;

[WolverineHandler]
public static class DeletePersonalityPresetHandler
{
    public static async Task<DeletePersonalityPreset.Result> Handle(DeletePersonalityPreset command, IUnitOfWork uow)
    {
        var repository = uow.GetRepository<IPersonalityPresetRepository>();
        var domain = await repository.TryFindAsync(command.Id);
        if (domain is null) return DeletePersonalityPreset.NotFound();
            
        repository.Delete(domain);
        await uow.SaveChangesAsync();
        
        return DeletePersonalityPreset.Success();
    }
}
