using Bunker.GameService.Hubs;
using Bunker.GameService.Persistence.Contracts;
using Bunker.GameService.Persistence.Contracts.Queries;
using Bunker.GameService.Persistence.Entities;
using Bunker.GameService.Transfers;
using Microsoft.AspNetCore.SignalR;
using Wolverine.Attributes;

namespace Bunker.GameService.Features.SendGameMessage;

[WolverineHandler]
public static class SendGameMessageHandler
{
    public static async Task<SendGameMessage.Result> Handle(
        SendGameMessage command,
        IGameQueries queries,
        IUnitOfWork uow,
        IHubContext<GameHub, IGameHub> hub)
    {
        var game = await queries.GetGameAsync(command.GameId);
        if (game is null)
            return SendGameMessage.Failure("Game not found.");

        var viewer = game.Participants.FirstOrDefault(p => p.AccountId == command.AccountId);
        if (viewer is null)
            return SendGameMessage.Failure("You are not in this game.");

        if (game.Status == "Finished")
            return SendGameMessage.Failure("Game is finished.");

        var message = new GameChatMessage
        {
            Id = Guid.NewGuid(),
            GameId = command.GameId,
            ParticipantId = viewer.Id,
            Nickname = viewer.Nickname,
            Text = command.Text,
            SentAt = DateTime.UtcNow
        };

        // Persist (Wolverine auto-commits via AutoApplyTransactions); then broadcast.
        uow.GetRepository<IGameChatMessageRepository>().Add(message);

        var dto = new ChatMessageDto(
            Id: message.Id.ToString(),
            ParticipantId: message.ParticipantId,
            Nickname: message.Nickname,
            Text: message.Text,
            SentAt: message.SentAt);

        await hub.Clients.Group(command.GameId.ToString()).ChatMessageReceived(dto);

        return SendGameMessage.Success(dto);
    }
}