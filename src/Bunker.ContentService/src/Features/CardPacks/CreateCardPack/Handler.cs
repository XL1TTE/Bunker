using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.CardPacks.CreateCardPack;

[WolverineHandler]
public static class CreateCardPackHandler
{
    public static async Task<CreateCardPack.Result> Handle(CreateCardPack command, IUnitOfWork uow)
    {
        var domain = CardPackFactory.New(command.Title, command.Description);
        foreach (var cardId in command.CardIds)
        {
            domain.AddCard(Card.Id.Create(cardId));
        }

        var repository = uow.GetRepository<ICardPackRepository>();
        repository.Add(domain);
        await uow.SaveChangesAsync();

        return CreateCardPack.Success(domain);
    }
}
