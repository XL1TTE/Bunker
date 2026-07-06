using Bunker.ContentService.Persistence.Contracts;
using Wolverine;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.PersonalityPresets.DeletePersonalityPreset;

[WolverineHandler]
public static class DeletePersonalityPresetHandler
{
    public static async Task<DeletePersonalityPreset.Result> Handle(
        DeletePersonalityPreset command,
        IMessageContext messaging,
        IUnitOfWork uow)
    {
        var repository = uow.GetRepository<IPersonalityPresetRepository>();
        var domain = await repository.TryFindAsync(command.Id);
        if (domain is null) return DeletePersonalityPreset.NotFound();

        repository.Delete(domain);

        await messaging.PublishAsync(new Messages.PersonalityPresetDeleted(Id: domain.PublicId.Value));

        return DeletePersonalityPreset.Success();
    }
}
