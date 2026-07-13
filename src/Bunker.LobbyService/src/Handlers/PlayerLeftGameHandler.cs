using Bunker.LobbyService.Features.LeaveLobby;
using Bunker.LobbyService.Messages;
using Humanizer;
using Serilog;
using System.Data.Common;
using Wolverine;
using Wolverine.Attributes;
using Wolverine.ErrorHandling;
using Wolverine.Runtime.Handlers;

namespace Bunker.LobbyService.Handlers;

[WolverineHandler]
public static class PlayerLeftGameHandler
{
    public static void Configure(HandlerChain chain)
    {
        chain.OnException<DbException>()
            .ScheduleRetry(1.Seconds(), 5.Seconds(), 15.Seconds()).WithBoundedJitter(0.25)
            .Then
            .MoveToErrorQueue();
    }

    public static async Task Handle(PlayerLeftGame message, IMessageContext messaging)
    {
        var result = await messaging.InvokeAsync<LeaveLobby.Result>(
            new LeaveLobby(LobbyId: message.LobbyId.ToString(), CallerId: message.AccountId));

        if (result is LeaveLobby.Result.Failure failure)
            Log.Warning("PlayerLeftGame for lobby {LobbyId} game {GameId} failed: {Error}",
                message.LobbyId, message.GameId, failure.Error);
    }
}