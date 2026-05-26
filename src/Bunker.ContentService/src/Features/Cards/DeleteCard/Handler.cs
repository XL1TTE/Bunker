using Bunker.ContentService.Domain;
using Wolverine.Attributes;
using Microsoft.EntityFrameworkCore;
using Bunker.ContentService.Persistence;
using Bunker.ContentService.Persistence.Contracts;

namespace Bunker.ContentService.Features.Cards.DeleteCard;

[WolverineHandler]
public static class DeleteCardHandler
{
    public static async Task<DeleteCard.Result> Handle(
        DeleteCard command,
        IUnitOfWork uow)
    {
        var repository = uow.GetRepository<ICardRepository>();
        var card = await repository.TryFindAsync(command.Id);

        if (card == null) return DeleteCard.NotFound();

        repository.Delete(card);
        await uow.SaveChangesAsync();

        return DeleteCard.Success();
    }
}
