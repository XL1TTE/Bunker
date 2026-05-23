using Bunker.ContentService.Domain;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Cards.CreateSexCard;

[WolverineHandler]
public static class CreateSexCardHandler
{
    public static async Task<CreateSexCard.Result> Handle(CreateSexCard command)
    {
        var card = Card.Create.SexCard(command.Sex);
        return CreateSexCard.Success(card);
    }
}
