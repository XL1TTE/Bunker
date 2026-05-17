using Bunker.Domain.Identity;
using Bunker.Domain.Lobbies;
using Bunker.Infrastructure.Identity;
using LobbyService.Persistence;
using LobbyService.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;
using Shared.Monads.Result;
using Wolverine.Attributes;

namespace LobbyService.Features.ToggleReady;

public record ToggleReady(Guid LobbyId, Guid UserId);

[WolverineHandler]
public static class ToggleReadyHandler
{
    public static async Task<ToggleReadyResult> Handle(
        ToggleReady command, 
        LobbyDbContext db)
    {
        var entity = await db.Lobbies
            .Include(x => x.Participants)
            .Include(x => x.Packs)
            .FirstOrDefaultAsync(x => x.Id == command.LobbyId);

        if (entity is null)
            return ToggleReadyResult.NotFound();

        var lobby = LobbyMapper.ToDomain(entity);
        var result = lobby.ToggleReady(PlayerIdFactory.Create(command.UserId));

        return await result.Match(
            onSuccess: async updatedLobby => 
            {
                entity.ApplyUpdate(updatedLobby);
                await db.SaveChangesAsync();
                return ToggleReadyResult.Success();
            },
            onFailure: error => Task.FromResult(error switch
            {
                LobbyErrors.PlayerNotInLobby => ToggleReadyResult.PlayerNotInLobby(),
                _ => throw new InvalidOperationException("Unexpected error type.")
            })
        );
    }
}
