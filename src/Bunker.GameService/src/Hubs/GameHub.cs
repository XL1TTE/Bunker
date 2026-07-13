using Bunker.GameService.Transfers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Bunker.GameService.Hubs;

public interface IGameHub
{
    // Phase 2b events:
    Task BunkerCardRevealed(BunkerCardDto bunkerCard);
    Task PhaseChanged(string phase, int roundNumber, int phaseDurationSeconds);
    Task TurnChanged(string participantId, string phase, int turnIndex, int turnDurationSeconds);
    Task AttributeRevealed(string participantId, string kind, string value);

    // Phase 2c events:
    Task ChatMessageReceived(ChatMessageDto message);
    Task VoteCast(string participantId);
    Task Eliminated(string participantId, TallyDto tally);
    Task RouletteStarted(IReadOnlyList<string> tiedParticipantIds);
    Task RouletteResult(string eliminatedId);
    Task GameFinished(IReadOnlyList<string> survivorParticipantIds);
}

[Authorize]
public sealed class GameHub : Hub<IGameHub>
{
    public Task JoinGame(string gameId) => Groups.AddToGroupAsync(Context.ConnectionId, gameId);

    public Task LeaveGame(string gameId) => Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);

    public override async Task OnConnectedAsync()
    {
        var gameId = Context.GetHttpContext()?.Request.Query["gameId"].ToString();
        if (!string.IsNullOrEmpty(gameId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var gameId = Context.GetHttpContext()?.Request.Query["gameId"].ToString();
        if (!string.IsNullOrEmpty(gameId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
        }

        await base.OnDisconnectedAsync(exception);
    }
}