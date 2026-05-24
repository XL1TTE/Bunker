using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Cards.UpdateProfessionCard;

[WolverineHandler]
public static class UpdateProfessionCardHandler
{
    public static async Task<UpdateProfessionCard.Result> Handle(UpdateProfessionCard command, IUnitOfWork uow)
    {
        var repository = uow.GetRepository<Card, Card.Id>();
        var card = await repository.TryFindAsync(command.Id);

        if (card is not ProfessionCard professionCard) return UpdateProfessionCard.NotFound();
        var update = professionCard.WithProfession(command.Profession);

        repository.Update(update);
        await uow.SaveChangesAsync();

        return UpdateProfessionCard.Success(update);
    }
}
