using Bunker.ContentService.Domain;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Cards.UpdateProfessionCard;

[WolverineHandler]
public static class UpdateProfessionCardHandler
{
    public static async Task<UpdateProfessionCard.Result> Handle(UpdateProfessionCard command)
    {
        var card = new ProfessionCard(command.Id, command.Profession);
        return UpdateProfessionCard.Success(card);
    }
}
