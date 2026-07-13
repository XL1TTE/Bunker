using Bunker.LobbyService.Domain;
using Bunker.LobbyService.Features.LeaveLobby.Events;
using Bunker.LobbyService.Hubs;
using Bunker.LobbyService.Persistence;
using Bunker.LobbyService.Persistence.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Shared.Monads.Result;
using Wolverine;
using Wolverine.Attributes;

namespace Bunker.LobbyService.Features.LeaveLobby;

[WolverineHandler]
public static class LeaveLobbyHandler
{
    public static async Task<LeaveLobby.Result> Handle(
        LeaveLobby command,
        IUnitOfWork uow,
        IMessageContext messaging)
    {
        if (!Guid.TryParse(command.LobbyId, out var lobbyGuid))
            return LeaveLobby.Failure("Invalid lobby id.");

        var callerId = AccountId.Create(command.CallerId);
        var lobbiesRepository = uow.GetRepository<ILobbyRepository>();

        var lobby = await lobbiesRepository.TryFindAsync(Lobby.Id.Restore(lobbyGuid));

        if (lobby is null)
            return LeaveLobby.Failure("Lobby not found.");

        var caller = lobby.Players.FirstOrDefault(p => p.UserId == callerId);
        if (caller is null)
            return LeaveLobby.Failure("You are not in this lobby.");

        if (caller.Role is Domain.Host)
        {
            return lobby.InGame
                ? await messaging.InvokeAsync<LeaveLobby.Result>(new HostLeavedFromGame(GameId: null, LobbyId: command.LobbyId, HostId: command.CallerId))
                : await messaging.InvokeAsync<LeaveLobby.Result>(new HostLeaved(HostId: command.CallerId, LobbyId: command.LobbyId));
        }

        return await messaging.InvokeAsync<LeaveLobby.Result>(new PlayerLeaved(LobbyId: command.LobbyId, PlayerId: caller.PublicId.Value.ToString()));
    }
}
