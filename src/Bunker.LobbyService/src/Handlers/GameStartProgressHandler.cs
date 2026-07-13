using Bunker.LobbyService.Hubs;
using Bunker.LobbyService.Messages;
using Microsoft.AspNetCore.SignalR;
using Wolverine.Attributes;

namespace Bunker.LobbyService.Handlers;

[WolverineHandler]
public static class GameStartProgressHandler
{
    public static Task Handle(
        GameStartProgress message,
        IHubContext<LobbyHub, ILobbyHub> hub)
    {
        return hub.Clients.Group(message.LobbyId.ToString()).GameStartProgress(message.Step, message.Status, message.Message);
    }
}