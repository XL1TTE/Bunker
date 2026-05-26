using Bunker.ContentService.Persistence.Contracts;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.CardPacks.DeleteCardPack;

[WolverineHandler]
public static class DeleteCardPackHandler
{
    public static async Task<DeleteCardPack.Result> Handle(DeleteCardPack command, IUnitOfWork uow)
    {
        var repository = uow.GetRepository<ICardPackRepository>();
        var domain = await repository.TryFindAsync(command.Id);
        if (domain is null) return DeleteCardPack.NotFound();

        repository.Delete(domain);
        await uow.SaveChangesAsync();
        
        return DeleteCardPack.Success();
    }
}
