using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Cards.CreateHobbiesCard;

[WolverineHandler]
public static class CreateHobbiesCardHandler
{
    public static async Task<CreateHobbiesCard.Result> Handle(
        CreateHobbiesCard command,
        IUnitOfWork uow)
    {
        var repository = uow.GetRepository<Card, Card.Id>();

        var card = HobbiesCard.CreateNew(command.Hobbies);

        repository.Add(card);
        await uow.SaveChangesAsync();

        return CreateHobbiesCard.Success(card);
    }
}
