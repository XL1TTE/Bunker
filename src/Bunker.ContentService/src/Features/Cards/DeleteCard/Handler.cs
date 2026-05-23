using Bunker.ContentService.Domain;
using Wolverine.Attributes;
using Microsoft.EntityFrameworkCore;
using Bunker.ContentService.Persistence;

namespace Bunker.ContentService.Features.Cards.DeleteCard;

[WolverineHandler]
public static class DeleteCardHandler
{
    public static async Task<DeleteCard.Result> Handle(DeleteCard command, ContentDbContext db)
    {
        var card = await db.Cards.FirstOrDefaultAsync(x => x.PublicId == command.Id.Value);
        if (card == null) return DeleteCard.NotFound();

        db.Cards.Remove(card);
        await db.SaveChangesAsync();

        return DeleteCard.Success();
    }
}