using Bunker.ContentService.Domain;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Cards.CreateAgeCard;

[WolverineHandler]
public static class CreateAgeCardHandler
{
    public static async Task<CreateAgeCard.Result> Handle(CreateAgeCard command)
    {
        var card = Card.Create.AgeCard(command.Age);
        return CreateAgeCard.Success(card);
    }
}
