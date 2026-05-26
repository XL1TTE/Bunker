using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Cards.CreateAgeCard;

[WolverineHandler]
public static class CreateAgeCardHandler
{
    public static async Task<CreateAgeCard.Result> Handle(
        CreateAgeCard command,
        IUnitOfWork uow)
    {
        var repository = uow.GetRepository<IAgeCardRepository>();

        var card = AgeCard.CreateNew(command.Age);

        repository.Add(card);
        await uow.SaveChangesAsync();

        return CreateAgeCard.Success(card);
    }
}
