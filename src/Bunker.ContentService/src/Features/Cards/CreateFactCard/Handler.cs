using Bunker.ContentService.Domain;
using Wolverine.Attributes;

namespace Bunker.ContentService.Features.Cards.CreateFactCard;

[WolverineHandler]
public static class CreateFactCardHandler
{
    public static async Task<CreateFactCard.Result> Handle(CreateFactCard command)
    {
        var card = new FactCard(Card.Id.New(), command.Fact);
        return CreateFactCard.Success(card);
    }
}
