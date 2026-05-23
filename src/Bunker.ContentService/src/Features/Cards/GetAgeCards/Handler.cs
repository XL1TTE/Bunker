using Bunker.ContentService.Domain;
using Wolverine.Attributes;
using Microsoft.EntityFrameworkCore;
using Bunker.ContentService.Persistence;

namespace Bunker.ContentService.Features.Cards.GetAgeCards;

[WolverineHandler]
public static class GetAgeCardsHandler
{
    public static async Task<GetAgeCards.Result> Handle(GetAgeCards query, ContentDbContext db)
    {
        var cards = await db.Cards.OfType<AgeCard>().ToListAsync();
        return GetAgeCards.Success(cards);
    }
}