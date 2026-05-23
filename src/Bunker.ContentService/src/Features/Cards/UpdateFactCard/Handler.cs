using Bunker.ContentService.Domain;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Cards.UpdateFactCard;

[WolverineHandler]
public static class UpdateFactCardHandler
{
    public static async Task<UpdateFactCard.Result> Handle(UpdateFactCard command)
    {
        var card = new FactCard(command.Id, command.Fact);
        return UpdateFactCard.Success(card);
    }
}
