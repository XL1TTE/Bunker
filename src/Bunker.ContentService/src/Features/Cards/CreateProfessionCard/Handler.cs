using Bunker.ContentService.Domain;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Cards.CreateProfessionCard;

[WolverineHandler]
public static class CreateProfessionCardHandler
{
    public static async Task<CreateProfessionCard.Result> Handle(CreateProfessionCard command)
    {
        var card = Card.Create.ProfessionCard(command.Profession);
        return CreateProfessionCard.Success(card);
    }
}
