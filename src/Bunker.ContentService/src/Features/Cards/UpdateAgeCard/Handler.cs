using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Cards.UpdateAgeCard;

[WolverineHandler]
public static class UpdateAgeCardHandler
{
    public static async Task<UpdateAgeCard.Result> Handle(UpdateAgeCard command, IUnitOfWork uow)
    {
        var repository = uow.GetRepository<IAgeCardRepository>();
        var ageCard = await repository.TryFindAsync(command.Id);

        if (ageCard == null) return UpdateAgeCard.NotFound();
        var update = ageCard.WithAge(command.Age);

        repository.Update(update);
        await uow.SaveChangesAsync();

        return UpdateAgeCard.Success(update);
    }
}
