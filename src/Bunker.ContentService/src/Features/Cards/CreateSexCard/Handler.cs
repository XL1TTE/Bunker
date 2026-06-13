using System.Text.Json;
using Bunker.ContentService.Domain;
using Bunker.ContentService.Persistence.Contracts;
using Bunker.ContentService.Transfers;
using Wolverine;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Cards.CreateSexCard;

[WolverineHandler]
public static class CreateSexCardHandler
{
    public static async Task<CreateSexCard.Result> Handle(
        CreateSexCard command,
        IMessageContext messaging,
        ILogger<CreateSexCard> logger,
        IUnitOfWork uow)
    {
        var repository = uow.GetRepository<ISexCardRepository>();

        try
        {
            var card = SexCard.CreateNew(SexCard.SexEnum.ParseString(command.Sex));

            repository.Add(card);
            await messaging.PublishAsync(new Messages.SexCardUpdated(Card: card.ToTransferObject()));

            return CreateSexCard.Success(card);
        }
        catch (ArgumentException error)
        {
            logger.LogError($"Attempt to create invalid card happened inside handler: {JsonSerializer.Serialize(command)}");
            return CreateSexCard.BadRequest(error.Message);
        }
    }
}
