using Bunker.Api.Common.Identity;
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

        var found = await queries.GetByInviteCodeAsync(command.InviteCode);
        if (found.IsFailure)
            return JoinLobby.Failure("Lobby not found.");

        var lobby = found.Match(onSuccess: l => l, onFailure: _ => null!);

        return await AddPlayerAndBroadcast(lobby, callerId, command.Nickname, uow, hub);
    }

    public static async Task<JoinLobby.Result> Handle(
        JoinByPassword command,
        IUnitOfWork uow,
        ILobbyRepository repository,
        IHubContext<LobbyHub, ILobbyHub> hub)
    {
        var callerId = AccountId.Create(command.CallerId);

        var lobby = await repository.TryFindAsync(Domain.Lobby.Id.Restore(command.LobbyId));
        if (lobby is null)
            return JoinLobby.Failure("Lobby not found.");

        if (!string.IsNullOrEmpty(lobby.PrivacyPolicy.Password) && lobby.PrivacyPolicy.Password != command.Password)
            return JoinLobby.Failure("Wrong password.");

        return await AddPlayerAndBroadcast(lobby, callerId, command.Nickname, uow, hub);
    }

    private static async Task<JoinLobby.Result> AddPlayerAndBroadcast(
        Domain.Lobby lobby,
        AccountId callerId,
        string nickname,
        IUnitOfWork uow,
        IHubContext<LobbyHub, ILobbyHub> hub)
    {
        if (lobby.Players.Any(p => p.UserId == callerId))
            return JoinLobby.Failure("You already joined this lobby.");

        var addResult = lobby.AddPlayer(PlayerParticipant.New(callerId, lobby.PublicId, nickname, Role.Member));
        if (addResult.IsFailure)
            return JoinLobby.Failure(addResult.Match(onSuccess: _ => "", onFailure: e => e.ToString()));

        var repository = uow.GetRepository<ILobbyRepository>();
        await repository.UpdateAsync(lobby);

        try
        {
            await uow.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return JoinLobby.Failure("Failed to join lobby.");
        }

        var participant = lobby.Participants.First(p => p is PlayerParticipant pp && pp.UserId == callerId);
        await hub.Clients.Group(lobby.PublicId.Value.ToString()).ParticipantJoined(participant.ToTransfer());

        return JoinLobby.Success(lobby);
    }
}
