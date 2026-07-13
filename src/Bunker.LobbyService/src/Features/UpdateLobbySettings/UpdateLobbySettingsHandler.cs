using Bunker.LobbyService.Domain;
using Bunker.LobbyService.Hubs;
using Bunker.LobbyService.Persistence;
using Bunker.LobbyService.Persistence.Abstractions;
using Bunker.LobbyService.Transfers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Shared.Monads.Result;
using Wolverine.Attributes;

namespace Bunker.LobbyService.Features.UpdateLobbySettings;

[WolverineHandler]
public static class UpdateLobbySettingsHandler
{
    public static async Task<UpdateLobbySettings.Result> Handle(
        UpdateLobbySettings command,
        IUnitOfWork uow,
        IHubContext<LobbyHub, ILobbyHub> hub)
    {
        if (!Guid.TryParse(command.LobbyId, out var lobbyGuid))
            return UpdateLobbySettings.Failure("Invalid lobby id.");

        var callerId = AccountId.Create(command.CallerId);
        var repository = uow.GetRepository<ILobbyRepository>();

        var lobby = await repository.TryFindAsync(Domain.Lobby.Id.Restore(lobbyGuid));
        if (lobby is null)
            return UpdateLobbySettings.Failure("Lobby not found.");

        var caller = lobby.Players.FirstOrDefault(p => p.UserId == callerId);
        if (caller is null || caller.Role != Role.Host)
            return UpdateLobbySettings.Failure("Only the host can change lobby settings.");

        var capacity = command.Capacity ?? lobby.Capacity;
        var isVisible = command.IsPublic ?? lobby.PrivacyPolicy.IsVisible;
        var password = command.Password ?? lobby.PrivacyPolicy.Password;

        var packs = command.SelectedPackIds is null
            ? lobby.Packs.ToList()
            : command.SelectedPackIds
                .Select(id => new LobbyCardPack(CardPackId.Create(Guid.Parse(id)), lobby.PublicId))
                .ToList();

        var result = lobby.UpdateConfiguration(capacity, isVisible, password, packs);
        if (result.IsFailure)
            return UpdateLobbySettings.Failure(result.Match(onSuccess: _ => "", onFailure: e => e.ToString()));

        if (command.Name is not null)
            lobby.Name = LobbyName.Create(command.Name);

        try
        {
            await uow.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return UpdateLobbySettings.Failure("Failed to update lobby settings.");
        }

        await hub.Clients.Group(lobby.PublicId.Value.ToString()).SettingsChanged(lobby.ToTransfer());
        return UpdateLobbySettings.Success(lobby);
    }
}
