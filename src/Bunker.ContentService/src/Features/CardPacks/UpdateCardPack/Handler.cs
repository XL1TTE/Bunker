using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Wolverine;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.CardPacks.UpdateCardPack;

[WolverineHandler]
public static class UpdateCardPackHandler
{
    public static async Task<UpdateCardPack.Result> Handle(
        UpdateCardPack command,
        IMessageContext messaging,
        IUnitOfWork uow)
    {
        var repository = uow.GetRepository<ICardPackRepository>();
        var domain = await repository.TryFindAsync(command.Id);

        if (domain is null) return UpdateCardPack.NotFound();

        var update = new CardPack(command.Id, command.Title, command.Description);

        foreach(var cardId in command.CardIds)
        {
            update.AddCard(Card.Id.Create(cardId));
        }

        repository.Update(update);

        await messaging.PublishAsync(new Messages.CardPackUpdated(
            Id: update.PublicId.Value,
            Title: update.Title,
            Description: update.Description,
            CardIds: update.Cards.Select(c => c.CardId.Value)));

        return UpdateCardPack.Success(update);
    }
}
