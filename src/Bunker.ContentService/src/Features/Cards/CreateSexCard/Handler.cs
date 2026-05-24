using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Cards.CreateSexCard;

[WolverineHandler]
public static class CreateSexCardHandler
{
    public static async Task<CreateSexCard.Result> Handle(
        CreateSexCard command,
        IUnitOfWork uow)
    {
        var repository = uow.GetRepository<Card, Card.Id>();

        var card = SexCard.CreateNew(SexCard.SexEnum.ParseString(command.Sex));

        repository.Add(card);
        await uow.SaveChangesAsync();

        return CreateSexCard.Success(card);
    }
}
