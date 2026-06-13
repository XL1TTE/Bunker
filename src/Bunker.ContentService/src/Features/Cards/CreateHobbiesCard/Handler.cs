using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Bunker.ContentService.Transfers;
using Wolverine;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Cards.CreateHobbiesCard;

[WolverineHandler]
public static class CreateHobbiesCardHandler
{
    public static async Task<CreateHobbiesCard.Result> Handle(
        CreateHobbiesCard command,
        IMessageContext messaging,
        IUnitOfWork uow)
    {
        var repository = uow.GetRepository<Card, Card.Id>();

        var card = HobbiesCard.CreateNew(command.Hobbies);

        repository.Add(card);

        await messaging.PublishAsync(new Messages.HobbiesCardUpdated(Card: card.ToTransferObject()));
        return CreateHobbiesCard.Success(card);
    }
}
