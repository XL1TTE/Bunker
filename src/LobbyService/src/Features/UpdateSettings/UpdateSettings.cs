using Bunker.Domain.Bots;
using Bunker.Domain.Cards;
using Bunker.Domain.Identity;
using Bunker.Domain.Lobbies;
using LobbyService.Features.CreateLobby;
using LobbyService.Persistence;
using LobbyService.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;
using Shared.Monads.Result;
using Wolverine.Attributes;

namespace LobbyService.Features.UpdateSettings;

public record UpdateSettings(
    Guid LobbyId,
    Guid UserId,
    int Capacity,
    IEnumerable<BotRequest> Bots,
    IEnumerable<string> Packs,
    bool IsPublic);

[WolverineHandler]
public static class UpdateSettingsHandler
{
    public static async Task<UpdateSettingsResult> Handle(
        UpdateSettings command, 
        LobbyDbContext db)
    {
        var entity = await db.Lobbies
            .Include(x => x.Participants)
            .Include(x => x.Packs)
            .FirstOrDefaultAsync(x => x.Id == command.LobbyId);

        if (entity is null)
            return UpdateSettingsResult.NotFound();

        // Validate Host authority
        var participant = entity.Participants.FirstOrDefault(p => p.PlayerId == command.UserId);
        if (participant == null || participant.Role != (int)ParticipantRole.Host)
            return UpdateSettingsResult.Forbidden();

        var lobby = LobbyMapper.ToDomain(entity);
        
        // Handle visibility switch if needed
        if ((command.IsPublic && lobby is PrivateLobby) || (!command.IsPublic && lobby is PublicLobby))
        {
            lobby = lobby.SetVisibility(command.IsPublic);
        }

        var bots = command.Bots.Select(b => BotFactory.Create(b.Name, b.PersonalityPreset));
        var packs = command.Packs.Select(CardPackIdFactory.Create);

        var result = lobby.UpdateSettings(command.Capacity, bots, packs);

        return await result.Match(
            onSuccess: async updatedLobby => 
            {
                entity.ApplyUpdate(updatedLobby);
                await db.SaveChangesAsync();
                return UpdateSettingsResult.Success();
            },
            onFailure: error => Task.FromResult(error switch
            {
                LobbyErrors.CapacityTooSmall => UpdateSettingsResult.Failure("Capacity cannot be lower than current player count"),
                _ => throw new InvalidOperationException("Unexpected error type.")
            })
        );
    }
}
