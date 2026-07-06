using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Wolverine;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.PersonalityPresets.CreatePersonalityPreset;

[WolverineHandler]
public static class CreatePersonalityPresetHandler
{
    public static async Task<CreatePersonalityPreset.Result> Handle(
        CreatePersonalityPreset command,
        IMessageContext messaging,
        IUnitOfWork uow)
    {
        var domain = PersonalityPresetFactory.New(command.Title, command.Description);
        var repository = uow.GetRepository<IPersonalityPresetRepository>();

        repository.Add(domain);

        await messaging.PublishAsync(new Messages.PersonalityPresetUpdated(
            Id: domain.PublicId.Value,
            Title: domain.Title,
            Description: domain.Description));

        return CreatePersonalityPreset.Success(domain);
    }
}
