using Bunker.LobbyService.Domain;
using Bunker.LobbyService.Hubs;
using Bunker.LobbyService.Persistence;
using Bunker.LobbyService.Persistence.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Wolverine.Attributes;

namespace Bunker.LobbyService.Features.LeaveLobby.Events;

[WolverineHandler]
public static class PlayerLeavedHandler
{
    public static async Task<LeaveLobby.Result> Handle(
        PlayerLeaved @event,
        IUnitOfWork uow,
        IHubContext<LobbyHub, ILobbyHub> hub)
    {
        var lobbiesRepository = uow.GetRepository<ILobbyRepository>();
        var lobby = await lobbiesRepository.TryFindAsync(Lobby.Id.Restore(Guid.Parse(@event.LobbyId)));

        if (lobby is null) return LeaveLobby.Failure("Lobby not found.");

        var player = lobby.Participants.FirstOrDefault(x => x.PublicId == Player.Id.Restore(Guid.Parse(@event.PlayerId)));
        
        if(player is null) return LeaveLobby.Success();
        
        lobby.Participants.Remove(player);
        await hub.Clients.Group(lobby.PublicId.ToString()).ParticipantLeft(player.PublicId.ToString());

        await uow.SaveChangesAsync();
        
        return LeaveLobby.Success();
    }
}
