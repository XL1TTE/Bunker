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

        var lobby = await repository.TryFindAsync(Lobby.Id.Restore(lobbyGuid));

        if (lobby is null)
            return LeaveLobby.Failure("Lobby not found.");

        var leavingResult = lobby.Leave(callerId);

        leavingResult.Match(
            onSuccess: async leaver =>
            {
                if (leaver.Role == Role.Host)
                {
                    await repository.DeleteByIdAsync(lobby.PublicId);
                    await hub.Clients.Group(lobby.PublicId.Value.ToString()).LobbyDestroyed("HostLeft");
                    return LeaveLobby.Success();
                }
                else
                {
                    await repository.UpdateAsync(lobby);
                    await hub.Clients.Group(lobby.PublicId.Value.ToString()).ParticipantLeft(leaver.PublicId.Value.ToString());
                    return LeaveLobby.Success();
                }
            },
            onFailure: error => LeaveLobby.Failure(error.ToString())
        );

        if (leavingResult is null)
            return LeaveLobby.Failure("You are not in the lobby you want to leave.");

        var leaveResult = lobby.Leave(callerId);
        if (leaveResult.IsFailure)
            return LeaveLobby.Failure(leaveResult.Match(onSuccess: _ => "", onFailure: e => e.ToString()));


        try
        {
            await uow.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return LeaveLobby.Failure("Failed to leave lobby.");
        }

        return LeaveLobby.Success();
    }
}
