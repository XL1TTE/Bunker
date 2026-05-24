using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Cards.UpdateFactCard;

[WolverineHandler]
public static class UpdateFactCardHandler
{
    public static async Task<UpdateFactCard.Result> Handle(UpdateFactCard command, IUnitOfWork uow)
    {
        var repository = uow.GetRepository<Card, Card.Id>();
        var card = await repository.TryFindAsync(command.Id);

        if (card is not FactCard factCard) return UpdateFactCard.NotFound();
        var update = factCard.WithFact(command.Fact);

        repository.Update(update);
        await uow.SaveChangesAsync();

        return UpdateFactCard.Success(update);
    }
}
