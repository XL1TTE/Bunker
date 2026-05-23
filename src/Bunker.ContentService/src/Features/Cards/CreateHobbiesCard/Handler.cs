using Bunker.ContentService.Domain;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Cards.CreateHobbiesCard;

[WolverineHandler]
public static class CreateHobbiesCardHandler
{
    public static async Task<CreateHobbiesCard.Result> Handle(CreateHobbiesCard command)
    {
        var card = Card.Create.HobbiesCard(command.Hobbies);
        return CreateHobbiesCard.Success(card);
    }
}
