using Wolverine;
using Wolverine.Attributes;
using Bunker.ContentService.Persistence.Contracts;

namespace Bunker.ContentService.Features.Cards.DeleteCard;

[WolverineHandler]
public static class DeleteCardHandler
{
    public static async Task<DeleteCard.Result> Handle(
        DeleteCard command,
        IMessageContext messaging,
        IUnitOfWork uow)
    {
        var repository = uow.GetRepository<ICardRepository>();
        var card = await repository.TryFindAsync(command.Id);

        if (card == null) return DeleteCard.NotFound();

        repository.Delete(card);

        await messaging.PublishAsync(new Messages.CardDeleted(Id: command.Id.Value));

        return DeleteCard.Success();
    }
}
