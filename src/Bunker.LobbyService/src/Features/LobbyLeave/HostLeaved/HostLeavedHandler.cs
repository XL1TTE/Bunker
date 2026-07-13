using Bunker.LobbyService.Hubs;
using Bunker.LobbyService.Persistence;
using Bunker.LobbyService.Persistence.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Wolverine.Attributes;

namespace Bunker.LobbyService.Features.LeaveLobby.Events;

[WolverineHandler]
public static class HostLeavedHandler
{
    public static async Task<LeaveLobby.Result> Handle(
        HostLeaved @event,
        IUnitOfWork uow,
        IHubContext<LobbyHub, ILobbyHub> hub)
    {
        var lobbiesRepository = uow.GetRepository<ILobbyRepository>();
        await lobbiesRepository.DeleteByIdAsync(Domain.Lobby.Id.Restore(Guid.Parse(@event.LobbyId)));
        
        await hub.Clients.Group(@event.LobbyId).LobbyDestroyed("HostLeft");
        return LeaveLobby.Success();
    }
}
