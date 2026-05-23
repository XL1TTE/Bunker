using Bunker.ContentService.Domain;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Cards.UpdateAgeCard;

[WolverineHandler]
public static class UpdateAgeCardHandler
{
    public static async Task<UpdateAgeCard.Result> Handle(UpdateAgeCard command)
    {
        var card = new AgeCard(command.Id, command.Age);
        return UpdateAgeCard.Success(card);
    }
}
