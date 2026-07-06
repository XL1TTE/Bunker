using Bunker.LobbyService.Domain;
using Bunker.LobbyService.Hubs;
using Bunker.LobbyService.Messages;
using Bunker.LobbyService.Persistence;
using Bunker.LobbyService.Persistence.Abstractions;
using Humanizer;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using System.Data.Common;
using Wolverine.Attributes;
using Wolverine.ErrorHandling;
using Wolverine.Runtime.Handlers;

namespace Bunker.LobbyService.Handlers;

[WolverineHandler]
public static class GameStartResultHandler
{
    public static void Configure(HandlerChain chain)
    {
        chain.OnException<DbException>()
            .ScheduleRetry(1.Seconds(), 5.Seconds(), 15.Seconds()).WithBoundedJitter(0.25)
            .Then
            .MoveToErrorQueue();
    }

    public static async Task Handle(
        GameStartSucceeded message,
        IUnitOfWork uow,
        IHubContext<LobbyHub, ILobbyHub> hub)
    {
        var repository = uow.GetRepository<ILobbyRepository>();
        var lobby = await repository.TryFindAsync(Domain.Lobby.Id.Restore(message.LobbyId));
        if (lobby is null)
        {
            Log.Warning("GameStartSucceeded for unknown lobby {LobbyId}", message.LobbyId);
            return;
        }

        lobby.MarkInGame();
        await repository.UpdateAsync(lobby);

        await hub.Clients.Group(message.LobbyId.ToString()).HandoffStarted(message.GameId.ToString());
    }

    public static async Task Handle(
        GameStartFailed message,
        IUnitOfWork uow,
        IHubContext<LobbyHub, ILobbyHub> hub)
    {
        var repository = uow.GetRepository<ILobbyRepository>();
        var lobby = await repository.TryFindAsync(Domain.Lobby.Id.Restore(message.LobbyId));
        if (lobby is null)
        {
            Log.Warning("GameStartFailed for unknown lobby {LobbyId}", message.LobbyId);
            return;
        }

        lobby.RevertStarting();
        await repository.UpdateAsync(lobby);

        await hub.Clients.Group(message.LobbyId.ToString()).GameStartFailed(message.Reason);
    }
}