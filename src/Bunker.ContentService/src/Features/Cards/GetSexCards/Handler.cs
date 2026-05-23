using Bunker.ContentService.Domain;
using Wolverine.Attributes;
using Microsoft.EntityFrameworkCore;
using Bunker.ContentService.Persistence;

namespace Bunker.ContentService.Features.Cards.GetSexCards;

[WolverineHandler]
public static class GetSexCardsHandler
{
    public static async Task<GetSexCards.Result> Handle(GetSexCards query, ContentDbContext db)
    {
        var cards = await db.Cards.OfType<SexCard>().ToListAsync();
        return GetSexCards.Success(cards);
    }
}