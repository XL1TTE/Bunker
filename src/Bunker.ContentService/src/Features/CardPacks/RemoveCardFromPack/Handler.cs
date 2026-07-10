using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Wolverine;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.CardPacks.RemoveCardFromPack;

[WolverineHandler]
public static class RemoveCardFromPackHandler
{
    public static async Task<RemoveCardFromPack.Result> Handle(
        RemoveCardFromPack command,
        IMessageContext messaging,
        IUnitOfWork uow)
    {
        var repository = uow.GetRepository<ICardPackRepository>();
        var domain = await repository.TryFindAsync(command.CardPackId);

        if (domain is null) return RemoveCardFromPack.NotFound();

        domain.RemoveCard(command.CardId);

        repository.Update(domain);

        await messaging.PublishAsync(new Messages.CardPackUpdated(
            Id: domain.PublicId.Value,
            Title: domain.Title,
            Description: domain.Description,
            GenerationPrompt: domain.GenerationPrompt,
            CardIds: domain.Cards.Select(c => c.CardId.Value)));

        return RemoveCardFromPack.Success(domain);
    }
}
