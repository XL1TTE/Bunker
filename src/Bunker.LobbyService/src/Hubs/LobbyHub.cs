using Bunker.LobbyService.Transfers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Bunker.LobbyService.Hubs;

public interface ILobbyHub
{
    Task ParticipantJoined(Transfer.LobbyParticipant participant);
    Task ParticipantLeft(string participantId);
    Task ParticipantKicked(string participantId, string byHostId);
    Task BotAdded(Transfer.LobbyParticipant bot);
    Task BotRemoved(string participantId);
    Task SettingsChanged(Transfer.LobbySnapshot lobby);
    Task ReadinessChanged(string participantId, string status);
    Task ChatMessageReceived(Transfer.ChatMessage message);
    Task LobbyDestroyed(string reason);
    Task HandoffStarted(string gameSessionId);
    Task GameStartFailed(string reason);
}

[Authorize]
public sealed class LobbyHub : Hub<ILobbyHub>
{
    public Task JoinLobby(string lobbyId) => Groups.AddToGroupAsync(Context.ConnectionId, lobbyId);

    public Task LeaveLobby(string lobbyId) => Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyId);

    public override async Task OnConnectedAsync()
    {
        var lobbyId = Context.GetHttpContext()?.Request.Query["lobbyId"].ToString();
        if (!string.IsNullOrEmpty(lobbyId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var lobbyId = Context.GetHttpContext()?.Request.Query["lobbyId"].ToString();
        if (!string.IsNullOrEmpty(lobbyId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyId);
        }

        await base.OnDisconnectedAsync(exception);
    }
}