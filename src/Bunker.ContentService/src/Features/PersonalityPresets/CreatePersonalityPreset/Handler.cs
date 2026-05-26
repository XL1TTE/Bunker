using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.PersonalityPresets.CreatePersonalityPreset;

[WolverineHandler]
public static class CreatePersonalityPresetHandler
{
    public static async Task<CreatePersonalityPreset.Result> Handle(CreatePersonalityPreset command, IUnitOfWork uow)
    {
        var domain = PersonalityPresetFactory.New(command.Title, command.Description);
        var repository = uow.GetRepository<IPersonalityPresetRepository>();
        
        repository.Add(domain);
        await uow.SaveChangesAsync();
        
        return CreatePersonalityPreset.Success(domain);
    }
}
