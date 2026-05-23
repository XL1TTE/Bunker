using Bunker.ContentService.Domain;
using Wolverine.Attributes;
using Microsoft.EntityFrameworkCore;
using Bunker.ContentService.Persistence;

namespace Bunker.ContentService.Features.Cards.GetHobbiesCards;

[WolverineHandler]
public static class GetHobbiesCardsHandler
{
    public static async Task<GetHobbiesCards.Result> Handle(GetHobbiesCards query, ContentDbContext db)
    {
        var cards = await db.Cards.OfType<HobbiesCard>().ToListAsync();
        return GetHobbiesCards.Success(cards);
    }
}