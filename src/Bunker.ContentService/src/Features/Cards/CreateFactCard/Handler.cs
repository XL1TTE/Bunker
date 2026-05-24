using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Cards.CreateFactCard;

[WolverineHandler]
public static class CreateFactCardHandler
{
    public static async Task<CreateFactCard.Result> Handle(
        CreateFactCard command,
        IUnitOfWork uow)
    {
        var repository = uow.GetRepository<Card, Card.Id>();

        var card = FactCard.CreateNew(command.Fact);

        repository.Add(card);
        await uow.SaveChangesAsync();

        return CreateFactCard.Success(card);
    }
}
