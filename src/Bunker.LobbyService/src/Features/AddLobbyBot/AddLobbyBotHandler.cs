using Bunker.LobbyService.Domain;
using Bunker.LobbyService.Hubs;
using Bunker.LobbyService.Persistence;
using Bunker.LobbyService.Persistence.Abstractions;
using Bunker.LobbyService.Transfers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Shared.Monads.Result;
using Wolverine.Attributes;

namespace Bunker.LobbyService.Features.AddLobbyBot;

[WolverineHandler]
public static class AddLobbyBotHandler
{
    public static async Task<AddLobbyBot.Result> Handle(
        AddLobbyBot command,
        IUnitOfWork uow,
        IHubContext<LobbyHub, ILobbyHub> hub)
    {
        if (!Guid.TryParse(command.LobbyId, out var lobbyGuid))
            return AddLobbyBot.Failure("Invalid lobby id.");

        var callerId = AccountId.Create(command.CallerId);
        var repository = uow.GetRepository<ILobbyRepository>();

        var lobby = await repository.TryFindAsync(Domain.Lobby.Id.Restore(lobbyGuid));
        if (lobby is null)
            return AddLobbyBot.Failure("Lobby not found.");

        var caller = lobby.Players.FirstOrDefault(p => p.UserId == callerId);
        if (caller is null || caller.Role != Role.Host)
            return AddLobbyBot.Failure("Only the host can add bots.");

        var bot = BotParticipant.New(lobby.PublicId, command.Nickname, BotPersonalityId.Create(command.PersonalityPresetId));
        var addResult = lobby.AddBot(bot);
        if (addResult.IsFailure)
            return AddLobbyBot.Failure(addResult.Match(onSuccess: _ => "", onFailure: e => e.ToString()));

        await repository.UpdateAsync(lobby);
        try
        {
            await uow.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return AddLobbyBot.Failure("Failed to add bot.");
        }

        await hub.Clients.Group(lobby.PublicId.Value.ToString()).BotAdded(bot.ToTransfer());
        return AddLobbyBot.Success(lobby);
    }
}
