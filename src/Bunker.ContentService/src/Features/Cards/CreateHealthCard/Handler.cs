using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Bunker.ContentService.Transfers;
using Wolverine;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Cards.CreateHealthCard;

[WolverineHandler]
public static class CreateHealthCardHandler
{
    public static async Task<CreateHealthCard.Result> Handle(
        CreateHealthCard command,
        IMessageContext messaging,
        IUnitOfWork uow)
    {
        var repository = uow.GetRepository<Card, Card.Id>();

        var card = HealthCard.CreateNew(command.Health);

        repository.Add(card);

        await messaging.PublishAsync(new Messages.HealthCardUpdated(Card: card.ToTransferObject()));
        return CreateHealthCard.Success(card);
    }
}