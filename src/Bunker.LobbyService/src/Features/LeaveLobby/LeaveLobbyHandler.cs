using Bunker.LobbyService.Domain;
using Bunker.LobbyService.Hubs;
using Bunker.LobbyService.Persistence;
using Bunker.LobbyService.Persistence.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Shared.Monads.Result;
using Wolverine.Attributes;

namespace Bunker.LobbyService.Features.LeaveLobby;

[WolverineHandler]
public static class LeaveLobbyHandler
{
    public static async Task<LeaveLobby.Result> Handle(
        LeaveLobby command,
        IUnitOfWork uow,
        IHubContext<LobbyHub, ILobbyHub> hub)
    {
        if (!Guid.TryParse(command.LobbyId, out var lobbyGuid))
            return LeaveLobby.Failure("Invalid lobby id.");

        var callerId = AccountId.Create(command.CallerId);
        var repository = uow.GetRepository<ILobbyRepository>();

        var lobby = await repository.TryFindAsync(Domain.Lobby.Id.Restore(lobbyGuid));
        if (lobby is null)
            return LeaveLobby.Failure("Lobby not found.");

        var leaving = lobby.Participants.FirstOrDefault(p => p is PlayerParticipant pp && pp.UserId == callerId);
        var leavingId = leaving?.PublicId.Value.ToString();

        var leaveResult = lobby.Leave(callerId);
        if (leaveResult.IsFailure)
            return LeaveLobby.Failure(leaveResult.Match(onSuccess: _ => "", onFailure: e => e.ToString()));

        var outcome = leaveResult.Match(onSuccess: o => o, onFailure: _ => new LeaveOutcome(false));
        var groupId = lobby.PublicId.Value.ToString();

        if (outcome.Destroyed)
        {
            await repository.DeleteByIdAsync(lobby.PublicId);
            try
            {
                await uow.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return LeaveLobby.Failure("Failed to leave lobby.");
            }

            await hub.Clients.Group(groupId).LobbyDestroyed("HostLeft");
            return LeaveLobby.Success();
        }

        await repository.UpdateAsync(lobby);
        try
        {
            await uow.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return LeaveLobby.Failure("Failed to leave lobby.");
        }

        await hub.Clients.Group(groupId).ParticipantLeft(leavingId!);
        return LeaveLobby.Success();
    }
}
