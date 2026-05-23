using Bunker.ContentService.Domain;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Cards.UpdateHobbiesCard;

[WolverineHandler]
public static class UpdateHobbiesCardHandler
{
    public static async Task<UpdateHobbiesCard.Result> Handle(UpdateHobbiesCard command)
    {
        var card = new HobbiesCard(command.Id, command.Hobbies);
        return UpdateHobbiesCard.Success(card);
    }
}
