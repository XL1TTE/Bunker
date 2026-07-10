using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Wolverine;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.CardPacks.AddCardToPack;

[WolverineHandler]
public static class AddCardToPackHandler
{
    public static async Task<AddCardToPack.Result> Handle(
        AddCardToPack command,
        IMessageContext messaging,
        IUnitOfWork uow)
    {
        var repository = uow.GetRepository<ICardPackRepository>();
        var domain = await repository.TryFindAsync(command.CardPackId);

        if (domain is null) return AddCardToPack.NotFound();

        domain.AddCard(command.CardId);

        repository.Update(domain);

        await messaging.PublishAsync(new Messages.CardPackUpdated(
            Id: domain.PublicId.Value,
            Title: domain.Title,
            Description: domain.Description,
            GenerationPrompt: domain.GenerationPrompt,
            CardIds: domain.Cards.Select(c => c.CardId.Value)));

        return AddCardToPack.Success(domain);
    }
}
