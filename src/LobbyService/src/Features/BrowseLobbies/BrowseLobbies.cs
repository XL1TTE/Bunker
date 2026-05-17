using Bunker.Domain.Lobbies;
using LobbyService.Persistence;
using Microsoft.EntityFrameworkCore;
using Wolverine.Attributes;

namespace LobbyService.Features.BrowseLobbies;

public record BrowseLobbies;

[WolverineHandler]
public static class BrowseLobbiesHandler
{
    public static async Task<BrowseLobbiesResult> Handle(BrowseLobbies query, LobbyDbContext db)
    {
        var lobbies = await db.Lobbies
            .AsNoTracking()
            .Where(x => x.IsPublic)
            .Include(x => x.Participants)
            .Include(x => x.Packs)
            .Select(x => new LobbySummaryResponse(
                x.Id,
                x.Participants.First(p => p.Role == (int)ParticipantRole.Host).Nickname,
                x.Capacity,
                x.Participants.Count,
                x.Bots.Count,
                x.Packs.Select(p => p.PackId)
            ))
            .ToListAsync();

        return BrowseLobbiesResult.Success(lobbies);
    }
}
