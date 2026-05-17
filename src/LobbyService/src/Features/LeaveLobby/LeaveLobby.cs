using Bunker.Domain.Identity;
using Bunker.Domain.Lobbies;
using Bunker.Infrastructure.Identity;
using LobbyService.Persistence;
using LobbyService.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;
using Shared.Monads.Result;
using Wolverine.Attributes;

namespace LobbyService.Features.LeaveLobby;

public record LeaveLobby(Guid LobbyId, Guid UserId);

[WolverineHandler]
public static class LeaveLobbyHandler
{
    public static async Task<LeaveLobbyResult> Handle(
        LeaveLobby command, 
        LobbyDbContext db)
    {
        var entity = await db.Lobbies
            .Include(x => x.Participants)
            .Include(x => x.Packs)
            .FirstOrDefaultAsync(x => x.Id == command.LobbyId);

        if (entity is null)
            return LeaveLobbyResult.NotFound();

        var playerId = PlayerIdFactory.Create(command.UserId);
        var participant = entity.Participants.FirstOrDefault(p => p.PlayerId == command.UserId);
        
        if (participant == null)
            return LeaveLobbyResult.Success(); // Already left or not in lobby

        // Pre-game rule: if host leaves, disband
        if (participant.Role == (int)ParticipantRole.Host)
        {
            db.Lobbies.Remove(entity);
        }
        else 
        {
            var lobby = LobbyMapper.ToDomain(entity);
            var result = lobby.RemoveParticipant(playerId);
            
            await result.Match(
                onSuccess: async updatedLobby => 
                {
                    entity.ApplyUpdate(updatedLobby);
                },
                onFailure: _ => Task.CompletedTask // ParticipantNotFound handled above
            );
        }

        await db.SaveChangesAsync();
        return LeaveLobbyResult.Success();
    }
}
