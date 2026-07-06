using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Bunker.ContentService.Transfers;
using Wolverine;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Cards.UpdateHobbiesCard;

[WolverineHandler]
public static class UpdateHobbiesCardHandler
{
    public static async Task<UpdateHobbiesCard.Result> Handle(
        UpdateHobbiesCard command,
        IMessageContext messaging,
        IUnitOfWork uow)
    {
        var repository = uow.GetRepository<IHobbiesCardRepository>();
        var hobbiesCard = await repository.TryFindAsync(command.Id);

        if (hobbiesCard == null) return UpdateHobbiesCard.NotFound();
        var update = hobbiesCard.WithHobbies(command.Hobbies);

        repository.Update(update);

        await messaging.PublishAsync(new Messages.HobbiesCardUpdated(Card: update.ToTransferObject()));

        return UpdateHobbiesCard.Success(update);
    }
}
