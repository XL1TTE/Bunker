using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Bunker.ContentService.Transfers;
using Wolverine;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Cards.CreateFactCard;

[WolverineHandler]
public static class CreateFactCardHandler
{
    public static async Task<CreateFactCard.Result> Handle(
        CreateFactCard command,
        IMessageContext messaging,
        IUnitOfWork uow)
    {
        var repository = uow.GetRepository<Card, Card.Id>();

        var card = FactCard.CreateNew(command.Fact);

        repository.Add(card);

        await messaging.PublishAsync(new Messages.FactCardUpdated(Card: card.ToTransferObject()));
        return CreateFactCard.Success(card);
    }
}
