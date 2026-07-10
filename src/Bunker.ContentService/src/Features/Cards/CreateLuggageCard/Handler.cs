using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Bunker.ContentService.Transfers;
using Wolverine;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Cards.CreateLuggageCard;

[WolverineHandler]
public static class CreateLuggageCardHandler
{
    public static async Task<CreateLuggageCard.Result> Handle(
        CreateLuggageCard command,
        IMessageContext messaging,
        IUnitOfWork uow)
    {
        var repository = uow.GetRepository<Card, Card.Id>();

        var card = LuggageCard.CreateNew(command.Luggage);

        repository.Add(card);

        await messaging.PublishAsync(new Messages.LuggageCardUpdated(Card: card.ToTransferObject()));
        return CreateLuggageCard.Success(card);
    }
}