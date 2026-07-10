using Bunker.ContentService.Persistence.Contracts;
using Wolverine;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.BunkerCards.DeleteBunkerCard;

[WolverineHandler]
public static class DeleteBunkerCardHandler
{
    public static async Task<DeleteBunkerCard.Result> Handle(
        DeleteBunkerCard command,
        IMessageContext messaging,
        IUnitOfWork uow)
    {
        var repository = uow.GetRepository<IBunkerCardRepository>();
        var domain = await repository.TryFindAsync(command.Id);
        if (domain is null) return DeleteBunkerCard.NotFound();

        repository.Delete(domain);

        await messaging.PublishAsync(new Messages.BunkerCardDeleted(Id: domain.PublicId.Value));

        return DeleteBunkerCard.Success();
    }
}