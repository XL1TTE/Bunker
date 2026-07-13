using Bunker.LobbyService.Domain;
using Bunker.LobbyService.Hubs;
using Bunker.LobbyService.Persistence;
using Bunker.LobbyService.Persistence.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Shared.Monads.Result;
using Wolverine.Attributes;

namespace Bunker.LobbyService.Features.RemoveLobbyBot;

[WolverineHandler]
public static class RemoveLobbyBotHandler
{
    public static async Task<RemoveLobbyBot.Result> Handle(
        RemoveLobbyBot command,
        IUnitOfWork uow,
        IHubContext<LobbyHub, ILobbyHub> hub)
    {
        if (!Guid.TryParse(command.LobbyId, out var lobbyGuid))
            return RemoveLobbyBot.Failure("Invalid lobby id.");

        var callerId = AccountId.Create(command.CallerId);
        var repository = uow.GetRepository<ILobbyRepository>();

        var lobby = await repository.TryFindAsync(Domain.Lobby.Id.Restore(lobbyGuid));
        if (lobby is null)
            return RemoveLobbyBot.Failure("Lobby not found.");

        var caller = lobby.Players.FirstOrDefault(p => p.UserId == callerId);
        if (caller is null || caller.Role != Role.Host)
            return RemoveLobbyBot.Failure("Only the host can remove bots.");

        var result = lobby.RemoveBot(LobbyParticipant.Id.Restore(command.ParticipantId), callerId);
        if (result.IsFailure)
            return RemoveLobbyBot.Failure(result.Match(onSuccess: _ => "", onFailure: e => e.ToString()));

        try
        {
            await uow.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return RemoveLobbyBot.Failure("Failed to remove bot.");
        }

        await hub.Clients.Group(lobby.PublicId.Value.ToString()).BotRemoved(command.ParticipantId.ToString());
        return RemoveLobbyBot.Success();
    }
}
