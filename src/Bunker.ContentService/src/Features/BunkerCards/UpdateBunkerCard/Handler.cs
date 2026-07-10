using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Wolverine;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.BunkerCards.UpdateBunkerCard;

[WolverineHandler]
public static class UpdateBunkerCardHandler
{
    public static async Task<UpdateBunkerCard.Result> Handle(
        UpdateBunkerCard command,
        IMessageContext messaging,
        IUnitOfWork uow)
    {
        var repository = uow.GetRepository<IBunkerCardRepository>();
        var domain = await repository.TryFindAsync(command.Id);
        if (domain is null) return UpdateBunkerCard.NotFound();

        domain.UpdateCatastrophe(command.Catastrophe);
        domain.UpdateSurvivalDuration(command.SurvivalDuration);
        domain.UpdateBunkerEnvironment(command.BunkerEnvironment);

        repository.Update(domain);

        await messaging.PublishAsync(new Messages.BunkerCardUpdated(
            Id: domain.PublicId.Value,
            Catastrophe: domain.Catastrophe,
            SurvivalDuration: domain.SurvivalDuration,
            BunkerEnvironment: domain.BunkerEnvironment));

        return UpdateBunkerCard.Success(domain);
    }
}