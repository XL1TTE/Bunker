using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Bunker.ContentService.Transfers;
using Wolverine;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Cards.UpdateLuggageCard;

[WolverineHandler]
public static class UpdateLuggageCardHandler
{
    public static async Task<UpdateLuggageCard.Result> Handle(
        UpdateLuggageCard command,
        IMessageContext messaging,
        IUnitOfWork uow)
    {
        var repository = uow.GetRepository<ILuggageCardRepository>();
        var luggageCard = await repository.TryFindAsync(command.Id);

        if (luggageCard == null) return UpdateLuggageCard.NotFound();
        var update = luggageCard.WithLuggage(command.Luggage);

        repository.Update(update);

        await messaging.PublishAsync(new Messages.LuggageCardUpdated(Card: update.ToTransferObject()));

        return UpdateLuggageCard.Success(update);
    }
}