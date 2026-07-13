using Bunker.LobbyService.Domain;
using Bunker.LobbyService.Messages;
using Bunker.LobbyService.Persistence;
using Bunker.LobbyService.Persistence.Abstractions;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Data.Common;
using Wolverine.Attributes;
using Wolverine.ErrorHandling;
using Wolverine.Runtime.Handlers;

namespace Bunker.LobbyService.Handlers;

[WolverineHandler]
public static class GameFinishedHandler
{
    public static void Configure(HandlerChain chain)
    {
        chain.OnException<DbException>()
            .ScheduleRetry(1.Seconds(), 5.Seconds(), 15.Seconds()).WithBoundedJitter(0.25)
            .Then
            .MoveToErrorQueue();
    }

    public static async Task Handle(
        GameFinished message,
        IUnitOfWork uow)
    {
        var repository = uow.GetRepository<ILobbyRepository>();
        var lobby = await repository.TryFindAsync(Domain.Lobby.Id.Restore(message.LobbyId));
        if (lobby is null)
        {
            Log.Warning("GameFinished for unknown lobby {LobbyId}", message.LobbyId);
            return;
        }

        lobby.ReopenAfterGame();

        try
        {
            await uow.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return;
        }
    }
}
