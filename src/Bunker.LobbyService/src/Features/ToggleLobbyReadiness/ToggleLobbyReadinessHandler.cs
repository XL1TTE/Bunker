using Bunker.LobbyService.Domain;
using Bunker.LobbyService.Hubs;
using Bunker.LobbyService.Persistence;
using Bunker.LobbyService.Persistence.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Shared.Monads.Result;
using Wolverine.Attributes;

namespace Bunker.LobbyService.Features.ToggleLobbyReadiness;

[WolverineHandler]
public static class ToggleLobbyReadinessHandler
{
    public static async Task<ToggleLobbyReadiness.Result> Handle(
        ToggleLobbyReadiness command,
        IUnitOfWork uow,
        ILobbyRepository repository,
        IHubContext<LobbyHub, ILobbyHub> hub)
    {
        if (!Guid.TryParse(command.LobbyId, out var lobbyGuid))
            return ToggleLobbyReadiness.Failure("Invalid lobby id.");

        var callerId = AccountId.Create(command.CallerId);
        var lobby = await repository.TryFindAsync(Domain.Lobby.Id.Restore(lobbyGuid));
        if (lobby is null)
            return ToggleLobbyReadiness.Failure("Lobby not found.");

        var result = lobby.ToggleReady(callerId);
        if (result.IsFailure)
            return ToggleLobbyReadiness.Failure(result.Match(onSuccess: _ => "", onFailure: e => e.ToString()));

        var participant = lobby.Players.First(p => p.UserId == callerId);

        await repository.UpdateAsync(lobby);
        try
        {
            await uow.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return ToggleLobbyReadiness.Failure("Failed to toggle readiness.");
        }

        await hub.Clients.Group(lobby.PublicId.Value.ToString())
            .ReadinessChanged(participant.PublicId.Value.ToString(), participant.Status.ToString());

        return ToggleLobbyReadiness.Success(lobby);
    }
}