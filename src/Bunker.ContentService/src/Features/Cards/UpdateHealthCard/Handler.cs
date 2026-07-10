using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Bunker.ContentService.Transfers;
using Wolverine;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Cards.UpdateHealthCard;

[WolverineHandler]
public static class UpdateHealthCardHandler
{
    public static async Task<UpdateHealthCard.Result> Handle(
        UpdateHealthCard command,
        IMessageContext messaging,
        IUnitOfWork uow)
    {
        var repository = uow.GetRepository<IHealthCardRepository>();
        var healthCard = await repository.TryFindAsync(command.Id);

        if (healthCard == null) return UpdateHealthCard.NotFound();
        var update = healthCard.WithHealth(command.Health);

        repository.Update(update);

        await messaging.PublishAsync(new Messages.HealthCardUpdated(Card: update.ToTransferObject()));

        return UpdateHealthCard.Success(update);
    }
}