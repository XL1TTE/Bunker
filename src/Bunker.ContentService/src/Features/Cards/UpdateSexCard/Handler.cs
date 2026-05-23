using Bunker.ContentService.Domain;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Cards.UpdateSexCard;

[WolverineHandler]
public static class UpdateSexCardHandler
{
    public static async Task<UpdateSexCard.Result> Handle(UpdateSexCard command)
    {
        var card = new SexCard(command.Id, Enum.Parse<SexCard.SexEnum>(command.Sex, true));
        return UpdateSexCard.Success(card);
    }
}
