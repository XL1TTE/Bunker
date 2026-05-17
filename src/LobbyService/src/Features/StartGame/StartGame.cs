using Bunker.Domain.Lobbies;
using Bunker.Infrastructure.Identity;
using LobbyService.Persistence;
using LobbyService.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;
using Wolverine;
using Wolverine.Attributes;

namespace LobbyService.Features.StartGame;

public record StartGame(Guid LobbyId, Guid UserId);

[WolverineHandler]
public static class StartGameHandler
{
    public static async Task<StartGameResult> Handle(
        StartGame command, 
        LobbyDbContext db,
        IMessageBus bus)
    {
        var entity = await db.Lobbies
            .Include(x => x.Participants)
            .Include(x => x.Packs)
            .FirstOrDefaultAsync(x => x.Id == command.LobbyId);

        if (entity is null)
            return StartGameResult.NotFound();

        // Validate Host authority
        var participant = entity.Participants.FirstOrDefault(p => p.PlayerId == command.UserId);
        if (participant == null || participant.Role != (int)ParticipantRole.Host)
            return StartGameResult.Forbidden();

        var lobby = LobbyMapper.ToDomain(entity);
        
        if (!lobby.AllReady)
            return StartGameResult.NotReady();

        // Prepare the handoff message
        var message = new PrepareGameRequested(
            entity.Id,
            entity.Participants.Select(p => new PlayerConfiguration(p.PlayerId, p.Nickname)),
            entity.Bots.Select(b => new BotConfiguration(b.Name, b.PersonalityPreset)),
            entity.Packs.Select(p => p.PackId)
        );

        // Send to Game Service via RabbitMQ
        await bus.PublishAsync(message);

        return StartGameResult.Success();
    }
}
