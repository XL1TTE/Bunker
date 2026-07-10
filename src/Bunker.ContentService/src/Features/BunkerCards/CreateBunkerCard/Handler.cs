using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Wolverine;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.BunkerCards.CreateBunkerCard;

[WolverineHandler]
public static class CreateBunkerCardHandler
{
    public static async Task<CreateBunkerCard.Result> Handle(
        CreateBunkerCard command,
        IMessageContext messaging,
        IUnitOfWork uow)
    {
        var domain = BunkerCardFactory.New(command.Catastrophe, command.SurvivalDuration, command.BunkerEnvironment);
        var repository = uow.GetRepository<IBunkerCardRepository>();

        repository.Add(domain);

        await messaging.PublishAsync(new Messages.BunkerCardUpdated(
            Id: domain.PublicId.Value,
            Catastrophe: domain.Catastrophe,
            SurvivalDuration: domain.SurvivalDuration,
            BunkerEnvironment: domain.BunkerEnvironment));

        return CreateBunkerCard.Success(domain);
    }
}