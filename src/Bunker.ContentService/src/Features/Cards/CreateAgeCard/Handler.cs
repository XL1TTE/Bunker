using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Bunker.ContentService.Transfers;
using Wolverine;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Cards.CreateAgeCard;

[WolverineHandler]
public static class CreateAgeCardHandler
{
    public static async Task<CreateAgeCard.Result> Handle(
        CreateAgeCard command,
        IMessageContext messaging,
        IUnitOfWork uow)
    {
        var repository = uow.GetRepository<IAgeCardRepository>();

        var card = AgeCard.CreateNew(command.Age);

        repository.Add(card);

        await messaging.PublishAsync(new Messages.AgeCardUpdated(Card: card.ToTransferObject()));
        return CreateAgeCard.Success(card);
    }
}
