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
        var lobbiesRepository = uow.GetRepository<ILobbyRepository>();

        var lobby = await lobbiesRepository.TryFindAsync(Lobby.Id.Restore(lobbyGuid));

        if (lobby is null)
            return LeaveLobby.Failure("Lobby not found.");

        var leavingResult = lobby.Leave(callerId);

        var result = await leavingResult.MatchAsync(
            onSuccess: async leaver =>
            {
                if (leaver.Role == Role.Host)
                {
                    await lobbiesRepository.DeleteByIdAsync(lobby.PublicId);
                    await hub.Clients.Group(lobby.PublicId.Value.ToString()).LobbyDestroyed("HostLeft");
                    return LeaveLobby.Success();
                }
                else
                {
                    await lobbiesRepository.UpdateAsync(lobby);
                    await hub.Clients.Group(lobby.PublicId.Value.ToString()).ParticipantLeft(leaver.PublicId.Value.ToString());
                    return LeaveLobby.Success();
                }
            },
            onFailure: async error => LeaveLobby.Failure(error.ToString())
        );

        try
        {
            await uow.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return LeaveLobby.Failure("Failed to leave lobby.");
        }

        return result;
    }
}
