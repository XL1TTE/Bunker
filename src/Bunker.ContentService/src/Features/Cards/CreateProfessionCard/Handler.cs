using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Bunker.ContentService.Transfers;
using Wolverine;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Cards.CreateProfessionCard;

[WolverineHandler]
[Transactional]
public static class CreateProfessionCardHandler
{
    public static async Task<CreateProfessionCard.Result> Handle(
        CreateProfessionCard command,
        IMessageContext messaging,
        IUnitOfWork uow)
    {
        var repository = uow.GetRepository<Domain.Card, Domain.Card.Id>();

        var card = ProfessionCard.CreateNew(command.Profession);
        repository.Add(card);

        await messaging.PublishAsync(new Messages.ProfessionCardUpdated(Card: card.ToTransferObject()));
        return CreateProfessionCard.Success(card);
    }
}
