using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Bunker.ContentService.Transfers;
using Wolverine;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Cards.UpdateSexCard;

[WolverineHandler]
public static class UpdateSexCardHandler
{
    public static async Task<UpdateSexCard.Result> Handle(
        UpdateSexCard command,
        IMessageContext messaging,
        IUnitOfWork uow)
    {
        var repository = uow.GetRepository<ISexCardRepository>();
        var sexCard = await repository.TryFindAsync(command.Id);

        if (sexCard == null) return UpdateSexCard.NotFound();
        var update = sexCard.WithSex(command.Sex);

        repository.Update(update);

        await messaging.PublishAsync(new Messages.SexCardUpdated(Card: update.ToTransferObject()));

        return UpdateSexCard.Success(update);
    }
}
