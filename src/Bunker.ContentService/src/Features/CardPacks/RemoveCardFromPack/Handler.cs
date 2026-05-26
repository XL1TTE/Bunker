using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.CardPacks.RemoveCardFromPack;

[WolverineHandler]
public static class RemoveCardFromPackHandler
{
    public static async Task<RemoveCardFromPack.Result> Handle(RemoveCardFromPack command, IUnitOfWork uow)
    {
        var repository = uow.GetRepository<ICardPackRepository>();
        var domain = await repository.TryFindAsync(command.CardPackId);
            
        if (domain is null) return RemoveCardFromPack.NotFound();

        domain.RemoveCard(command.CardId);

        repository.Update(domain);
        await uow.SaveChangesAsync();

        return RemoveCardFromPack.Success(domain);
    }
}
