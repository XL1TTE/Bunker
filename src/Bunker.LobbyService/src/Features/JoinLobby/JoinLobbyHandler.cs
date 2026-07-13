using Bunker.LobbyService.Domain;
using Bunker.LobbyService.Hubs;
using Bunker.LobbyService.Persistence;
using Bunker.LobbyService.Persistence.Abstractions;
using Bunker.LobbyService.Persistence.Queries;
using Bunker.LobbyService.Transfers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Shared.Monads.Result;
using Wolverine.Attributes;

namespace Bunker.LobbyService.Features.JoinLobby;

[WolverineHandler]
public static class JoinLobbyHandler
{
    public static async Task<JoinLobby.Result> Handle(
        JoinByInviteCode command,
        IUnitOfWork uow,
        ILobbyQueries queries,
        IHubContext<LobbyHub, ILobbyHub> hub)
    {
        var callerId = AccountId.Create(command.CallerId);

        var normalized = (command.InviteCode ?? "").Trim().ToUpperInvariant();
        var repository = uow.GetRepository<ILobbyRepository>();
        var lobby = await repository.TryFindByInviteCodeAsync(InviteCode.Create(normalized));

        if (lobby is null)
            return JoinLobby.Failure("Lobby not found.");

        return await AddPlayerAndBroadcast(lobby, callerId, command.Nickname, uow, queries, hub);
    }

    public static async Task<JoinLobby.Result> Handle(
        JoinByPassword command,
        IUnitOfWork uow,
        ILobbyQueries queries,
        IHubContext<LobbyHub, ILobbyHub> hub)
    {
        var callerId = AccountId.Create(command.CallerId);

        var repository = uow.GetRepository<ILobbyRepository>();
        var lobby = await repository.TryFindAsync(Domain.Lobby.Id.Restore(command.LobbyId));

        if (lobby is null)
            return JoinLobby.Failure("Lobby not found.");

        if (!string.IsNullOrEmpty(lobby.PrivacyPolicy.Password) && lobby.PrivacyPolicy.Password != command.Password)
            return JoinLobby.Failure("Wrong password.");

        return await AddPlayerAndBroadcast(lobby, callerId, command.Nickname, uow, queries, hub);
    }

    private static async Task<JoinLobby.Result> AddPlayerAndBroadcast(
        Domain.Lobby lobby,
        AccountId callerId,
        string nickname,
        IUnitOfWork uow,
        ILobbyQueries queries,
        IHubContext<LobbyHub, ILobbyHub> hub)
    {
        if (lobby.Players.Any(p => p.UserId == callerId))
            return JoinLobby.Success(lobby);

        var existing = await queries.GetByPlayerIdAsync(callerId);
        if (existing is not null && existing.PublicId != lobby.PublicId)
            return JoinLobby.Failure("You're already in another lobby. Leave it before joining a new one.");

        var addResult = lobby.AddPlayer(Player.New(callerId, lobby.PublicId, nickname, Role.Member));
        if (addResult.IsFailure)
            return JoinLobby.Failure(addResult.Match(onSuccess: _ => "", onFailure: e => e.ToString()));

        var repository = uow.GetRepository<ILobbyRepository>();

        try
        {
            await uow.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return JoinLobby.Failure("Failed to join lobby.");
        }

        var participant = lobby.Participants.First(p => p is Player pp && pp.UserId == callerId);
        await hub.Clients.Group(lobby.PublicId.Value.ToString()).ParticipantJoined(participant.ToTransfer());

        return JoinLobby.Success(lobby);
    }
}
