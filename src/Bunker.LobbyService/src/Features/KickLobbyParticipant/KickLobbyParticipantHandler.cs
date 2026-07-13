using Bunker.LobbyService.Domain;
using Bunker.LobbyService.Hubs;
using Bunker.LobbyService.Persistence;
using Bunker.LobbyService.Persistence.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Shared.Monads.Result;
using Wolverine.Attributes;

namespace Bunker.LobbyService.Features.KickLobbyParticipant;

[WolverineHandler]
public static class KickLobbyParticipantHandler
{
    public static async Task<KickLobbyParticipant.Result> Handle(
        KickLobbyParticipant command,
        IUnitOfWork uow,
        IHubContext<LobbyHub, ILobbyHub> hub)
    {
        if (!Guid.TryParse(command.LobbyId, out var lobbyGuid))
            return KickLobbyParticipant.Failure("Invalid lobby id.");

        var callerId = AccountId.Create(command.CallerId);
        var repository = uow.GetRepository<ILobbyRepository>();

        var lobby = await repository.TryFindAsync(Domain.Lobby.Id.Restore(lobbyGuid));
        if (lobby is null)
            return KickLobbyParticipant.Failure("Lobby not found.");

        var host = lobby.Players.FirstOrDefault(p => p.Role == Role.Host);
        if (host is null || host.UserId != callerId)
            return KickLobbyParticipant.Failure("Only the host can kick participants.");

        var byHostId = host.PublicId.Value.ToString();
        var result = lobby.Kick(LobbyParticipant.Id.Restore(command.ParticipantId), callerId);
        if (result.IsFailure)
            return KickLobbyParticipant.Failure(result.Match(onSuccess: _ => "", onFailure: e => e.ToString()));

        try
        {
            await uow.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return KickLobbyParticipant.Failure("Failed to kick participant.");
        }

        await hub.Clients.Group(lobby.PublicId.Value.ToString()).ParticipantKicked(command.ParticipantId.ToString(), byHostId);
        return KickLobbyParticipant.Success();
    }
}
