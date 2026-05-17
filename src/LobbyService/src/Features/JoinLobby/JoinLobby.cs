using Bunker.Domain.Identity;
using Bunker.Domain.Lobbies;
using Bunker.Infrastructure.Identity;
using LobbyService.Persistence;
using LobbyService.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;
using Shared.Monads.Result;
using Wolverine.Attributes;

namespace LobbyService.Features.JoinLobby;

public record JoinLobby(
    Guid UserId,
    string Nickname,
    string InviteCode);

[WolverineHandler]
public static class JoinLobbyHandler
{
    public static async Task<JoinLobbyResult> Handle(
        JoinLobby command, 
        LobbyDbContext db)
    {
        var entity = await db.Lobbies
            .Include(x => x.Participants)
            .Include(x => x.Packs)
            .FirstOrDefaultAsync(x => x.InviteCode == command.InviteCode);

        if (entity is null)
            return JoinLobbyResult.NotFound();

        var lobby = LobbyMapper.ToDomain(entity);
        var participant = Bunker.Domain.Lobbies.PlayerFactory.Create(
            PlayerIdFactory.Create(command.UserId),
            command.Nickname,
            ParticipantRole.Member);

        var result = lobby.AddPlayer(participant);

        return await result.Match(
            onSuccess: async updatedLobby => 
            {
                entity.ApplyUpdate(updatedLobby);
                await db.SaveChangesAsync();
                return JoinLobbyResult.Success(new JoinLobbyResponse(entity.Id));
            },
            onFailure: error => Task.FromResult(error switch
            {
                LobbyErrors.PlayerAlreadyInLobby => JoinLobbyResult.AlreadyIn(),
                LobbyErrors.LobbyIsFull => JoinLobbyResult.Full(),
                _ => throw new InvalidOperationException("Unexpected error type.")
            })
        );
    }
}
