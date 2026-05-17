using Bunker.Domain.Bots;
using Bunker.Domain.Identity;
using Bunker.Domain.Lobbies;
using LobbyService.Persistence;
using LobbyService.Persistence.Mappers;
using Wolverine.Attributes;

namespace LobbyService.Features.CreateLobby;

public record CreateLobby(
    Guid UserId,
    string Nickname,
    int Capacity,
    IEnumerable<BotRequest> Bots,
    IEnumerable<Guid> Packs,
    bool IsPublic);

[WolverineHandler]
public static class CreateLobbyHandler
{
    public static async Task<CreateLobbyResult> Handle(
        CreateLobby command, 
        LobbyDbContext db)
    {
        var lobbyId = LobbyIdFactory.New();
        var inviteCode = InviteCodeFactory.New();

        var host = Bunker.Domain.Lobbies.PlayerFactory.Create(
            PlayerIdFactory.Create(command.UserId),
            command.Nickname,
            ParticipantRole.Host);

        try 
        {
            var bots = command.Bots.Select(b => BotFactory.Create(b.Name, b.PersonalityPreset));
            var packs = command.Packs.Select(CardPackIdFactory.Create);

            Lobby lobby = command.IsPublic
                ? LobbyFactory.CreatePublic(lobbyId, inviteCode, command.Capacity, host, bots, packs)
                : LobbyFactory.CreatePrivate(lobbyId, inviteCode, command.Capacity, host, bots, packs);

            var entity = LobbyMapper.ToEntity(lobby);

            db.Lobbies.Add(entity);
            await db.SaveChangesAsync();

            return CreateLobbyResult.Success(new CreateLobbyResponse(entity.Id, entity.InviteCode));
        }
        catch (ArgumentException ex)
        {
            return CreateLobbyResult.Failure(ex.Message);
        }
    }
}

