using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Cards.UpdateSexCard;

[WolverineHandler]
public static class UpdateSexCardHandler
{
    public static async Task<UpdateSexCard.Result> Handle(UpdateSexCard command, IUnitOfWork uow)
    {
        var repository = uow.GetRepository<Card, Card.Id>();
        var card = await repository.TryFindAsync(command.Id);

        if (card is not SexCard sexCard) return UpdateSexCard.NotFound();
        var update = sexCard.WithSex(command.Sex);

        repository.Update(update);
        await uow.SaveChangesAsync();

        return UpdateSexCard.Success(update);
    }
}
