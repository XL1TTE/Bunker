using Bunker.ContentService.Domain;
using Wolverine.Attributes;
using Microsoft.EntityFrameworkCore;
using Bunker.ContentService.Persistence;
using Bunker.ContentService.Persistence.Entities;

namespace Bunker.ContentService.Features.Cards.GetCardById;

[WolverineHandler]
public static class GetCardByIdHandler
{
    public static async Task<GetCardById.Result> Handle(GetCardById query, ContentDbContext db)
    {
        var card = await db.Cards.FirstOrDefaultAsync(x => x.PublicId == query.Id.Value);
        if (card == null) return GetCardById.NotFound();

        return GetCardById.Success(card.ToDomain());
    }
}