using Bunker.ContentService.Domain;
using Wolverine.Attributes;
using Microsoft.EntityFrameworkCore;
using Bunker.ContentService.Persistence;

namespace Bunker.ContentService.Features.Cards.GetProfessionCards;

[WolverineHandler]
public static class GetProfessionCardsHandler
{
    public static async Task<GetProfessionCards.Result> Handle(GetProfessionCards query, ContentDbContext db)
    {
        var cards = await db.Cards.OfType<ProfessionCard>().ToListAsync();
        return GetProfessionCards.Success(cards);
    }
}