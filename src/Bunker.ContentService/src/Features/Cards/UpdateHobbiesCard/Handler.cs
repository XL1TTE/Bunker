using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Cards.UpdateHobbiesCard;

[WolverineHandler]
public static class UpdateHobbiesCardHandler
{
    public static async Task<UpdateHobbiesCard.Result> Handle(UpdateHobbiesCard command, IUnitOfWork uow)
    {
        var repository = uow.GetRepository<Card, Card.Id>();
        var card = await repository.TryFindAsync(command.Id);

        if (card is not HobbiesCard hobbiesCard) return UpdateHobbiesCard.NotFound();
        var update = hobbiesCard.WithHobbies(command.Hobbies);

        repository.Update(update);
        await uow.SaveChangesAsync();

        return UpdateHobbiesCard.Success(update);
    }
}
