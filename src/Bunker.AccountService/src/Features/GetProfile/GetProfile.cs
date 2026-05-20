using Microsoft.EntityFrameworkCore;
using Wolverine.Attributes;
using Bunker.AccountService.Domain;
using Bunker.AccountService.Persistence;

namespace Bunker.AccountService.Features.GetProfile;

[WolverineHandler]
public static class GetProfileHandler
{
    public static async Task<GetProfileResult> Handle(GetProfile query, AccountDbContext db)
    {
        var guidId = Guid.Parse(query.Id);
        var player = await db.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PublicId == Account.Id.Restore(guidId));

        if (player is null) 
            return GetProfileResult.NotFound();

        var response = new PlayerProfileResponse(
            player.Nickname.Value,
            player.Stats.TotalGames,
            player.Stats.Wins,
            player.Stats.Losses);

        return GetProfileResult.Success(response);
    }
}
