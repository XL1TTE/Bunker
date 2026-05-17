using Bunker.Domain.Identity;
using PlayerService.Persistence;
using Microsoft.EntityFrameworkCore;
using Wolverine.Attributes;

namespace PlayerService.Features.GetProfile;

[WolverineHandler]
public static class GetProfileHandler
{
    public static async Task<GetProfileResult> Handle(GetProfile query, PlayerDbContext db)
    {
        var guidId = Guid.Parse(query.Id);
        var player = await db.Players
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == PlayerId.Create(guidId));

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
