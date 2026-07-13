using Bunker.LobbyService.Domain;
using Bunker.LobbyService.Hubs;
using Bunker.LobbyService.Persistence;
using Bunker.LobbyService.Persistence.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Shared.Monads.Result;
using Wolverine.Attributes;

namespace Bunker.LobbyService.Features.LeaveLobby.Events;

[WolverineHandler]
public static class HostLeavedFromGameHandler
{
    public static async Task<LeaveLobby.Result> Handle(
        HostLeavedFromGame @event,
        IUnitOfWork uow,
        IHubContext<LobbyHub, ILobbyHub> hub)
    {
        var lobbiesRepository = uow.GetRepository<ILobbyRepository>();
        var lobby = await lobbiesRepository.TryFindAsync(Domain.Lobby.Id.Restore(Guid.Parse(@event.LobbyId)));

        if (lobby is null) return LeaveLobby.Success();
    
        lobby.Participants.Remove(lobby.Host);
        var result = lobby.ReassignHost();
        
        return await result.MatchAsync(
            onSuccess: async host =>
            {
                try
                {
                    await uow.SaveChangesAsync();
                    await hub.Clients.Group(@event.LobbyId).HostChanged(host.PublicId.Value.ToString());
                    return LeaveLobby.Success();
                }
                catch
                {
                    return LeaveLobby.Failure("Failed to leave lobby.");
                }
            },
            onFailure: async error => LeaveLobby.Failure(error)
        );
        
    }
}
