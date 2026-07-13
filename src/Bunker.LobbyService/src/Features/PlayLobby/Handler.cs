using Bunker.LobbyService.Domain;
using Bunker.LobbyService.Hubs;
using Bunker.LobbyService.Messages;
using Bunker.LobbyService.Persistence;
using Bunker.LobbyService.Persistence.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Shared.Monads.Result;
using Wolverine;
using Wolverine.Attributes;

namespace Bunker.LobbyService.Features.PlayLobby;

[WolverineHandler]
public static class PlayLobbyHandler
{
    public static async Task<PlayLobby.Result> Handle(
        PlayLobby command,
        IMessageContext messaging,
        IUnitOfWork uow,
        IHubContext<LobbyHub, ILobbyHub> hub)
    {
        if (!Guid.TryParse(command.LobbyId, out var lobbyGuid))
            return PlayLobby.Failure("Invalid lobby id.");

        var repository = uow.GetRepository<ILobbyRepository>();
        var lobby = await repository.TryFindAsync(Domain.Lobby.Id.Restore(lobbyGuid));
        if (lobby is null)
            return PlayLobby.Failure("Lobby not found.");

        var startOutcome = lobby.StartGame(AccountId.Create(command.HostId)).Match(
            onSuccess: l => (Error: (string?)null, Lobby: (Domain.Lobby?)l),
            onFailure: e => (Error: e.ToString(), Lobby: (Domain.Lobby?)null));

        if (startOutcome.Error is not null)
            return PlayLobby.Failure(startOutcome.Error);

        var started = startOutcome.Lobby!;

        await hub.Clients.Group(started.PublicId.Value.ToString())
            .GameStartProgress("validate-lobby", "Succeeded", null);

        var participants = started.Participants.Select(p => new SagaParticipant(
            Id: p.PublicId.Value.ToString(),
            Nickname: p.Nickname,
            Type: p is BotParticipant ? "Bot" : "Player",
            PersonalityPresetId: p is BotParticipant bot ? bot.PersonalityPresetId.Value : null,
            AccountId: p is Player player ? player.UserId.Value : null
        )).ToList();

        await messaging.PublishAsync(new GameStartRequested(
            StartRequestId: Guid.NewGuid(),
            LobbyId: started.PublicId.Value,
            HostId: command.HostId,
            CardPackIds: started.Packs.Select(x => x.PackId.Value).ToList(),
            PersonalityPresetIds: started.Bots.Select(b => b.PersonalityPresetId.Value).ToList(),
            Participants: participants
        ));

        return PlayLobby.Success();
    }
}
