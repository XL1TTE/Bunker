using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Cards.CreateProfessionCard;

[WolverineHandler]
public static class CreateProfessionCardHandler
{
    public static async Task<CreateProfessionCard.Result> Handle(
        CreateProfessionCard command,
        IUnitOfWork uow)
    {
        var repository = uow.GetRepository<Domain.Card, Domain.Card.Id>();
        var card = ProfessionCard.CreateNew(command.Profession);

        repository.Add(card);
        await uow.SaveChangesAsync();

        return CreateProfessionCard.Success(card);
    }
}
