using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Wolverine;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.CardPacks.CreateCardPack;

[WolverineHandler]
public static class CreateCardPackHandler
{
    public static async Task<CreateCardPack.Result> Handle(
        CreateCardPack command,
        IMessageContext messaging,
        IUnitOfWork uow)
    {
        var domain = CardPackFactory.New(command.Title, command.Description, command.GenerationPrompt);
        foreach (var cardId in command.CardIds)
        {
            domain.AddCard(Card.Id.Create(cardId));
        }

        var repository = uow.GetRepository<ICardPackRepository>();
        repository.Add(domain);

        await messaging.PublishAsync(new Messages.CardPackUpdated(
            Id: domain.PublicId.Value,
            Title: domain.Title,
            Description: domain.Description,
            GenerationPrompt: domain.GenerationPrompt,
            CardIds: domain.Cards.Select(c => c.CardId.Value)));

        return CreateCardPack.Success(domain);
    }
}
