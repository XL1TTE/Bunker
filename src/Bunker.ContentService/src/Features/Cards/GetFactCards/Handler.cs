using Bunker.ContentService.Domain;
using Wolverine.Attributes;
using Microsoft.EntityFrameworkCore;
using Bunker.ContentService.Persistence;

namespace Bunker.ContentService.Features.Cards.GetFactCards;

[WolverineHandler]
public static class GetFactCardsHandler
{
    public static async Task<GetFactCards.Result> Handle(GetFactCards query, ContentDbContext db)
    {
        var cards = await db.Cards.OfType<FactCard>().ToListAsync();
        return GetFactCards.Success(cards);
    }
}