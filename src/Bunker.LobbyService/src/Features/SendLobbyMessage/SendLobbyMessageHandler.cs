using Bunker.LobbyService.Domain;
using Bunker.LobbyService.Hubs;
using Bunker.LobbyService.Persistence;
using Bunker.LobbyService.Persistence.Abstractions;
using Bunker.LobbyService.Transfers;
using Microsoft.AspNetCore.SignalR;
using Wolverine.Attributes;

namespace Bunker.LobbyService.Features.SendLobbyMessage;

[WolverineHandler]
public static class SendLobbyMessageHandler
{
    public static async Task<SendLobbyMessage.Result> Handle(
        SendLobbyMessage command,
        IUnitOfWork uow,
        IHubContext<LobbyHub, ILobbyHub> hub)
    {
        if (!Guid.TryParse(command.LobbyId, out var lobbyGuid))
            return SendLobbyMessage.Failure("Invalid lobby id.");

        var callerId = AccountId.Create(command.CallerId);

        var repository = uow.GetRepository<ILobbyRepository>();
        var lobby = await repository.TryFindAsync(Domain.Lobby.Id.Restore(lobbyGuid));

        if (lobby is null)
            return SendLobbyMessage.Failure("Lobby not found.");

        var participant = lobby.Participants.FirstOrDefault(p => p is Player pp && pp.UserId == callerId);
        if (participant is null)
            return SendLobbyMessage.Failure("You are not in this lobby.");

        var message = new Transfer.ChatMessage(
            Id: Guid.NewGuid().ToString(),
            ParticipantId: participant.PublicId.Value.ToString(),
            Nickname: participant.Nickname,
            Text: command.Text,
            SentAt: DateTime.UtcNow
        );

        await hub.Clients.Group(lobby.PublicId.Value.ToString()).ChatMessageReceived(message);
        return SendLobbyMessage.Success(message);
    }
}
