using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.CardPacks.AddCardToPack;

[WolverineHandler]
public static class AddCardToPackHandler
{
    public static async Task<AddCardToPack.Result> Handle(AddCardToPack command, IUnitOfWork uow)
    {
        var repository = uow.GetRepository<ICardPackRepository>();
        var domain = await repository.TryFindAsync(command.CardPackId);
            
        if (domain is null) return AddCardToPack.NotFound();

        domain.AddCard(command.CardId);

        repository.Update(domain);
        await uow.SaveChangesAsync();

        return AddCardToPack.Success(domain);
    }
}
