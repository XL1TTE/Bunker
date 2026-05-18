
using Bunker.Domain.Lobbies;
using Bunker.Persistence.Mappers;
using Riok.Mapperly.Abstractions;

namespace LobbyService.Persistence.Mappers;

[Mapper]
[UseStaticMapper(typeof(CardPackMapper))]
[UseStaticMapper(typeof(CardPackMapperExtensions))]
[UseStaticMapper(typeof(LobbyParticipantMapper))]
[UseStaticMapper(typeof(LobbyMapperExtensions))]
[UseStaticMapper(typeof(PrivacyPolicyMapper))]
public static partial class LobbyMapper
{
    [MapperIgnoreSource(nameof(Entities.Lobby.Status))]
    public static partial Lobby ToDomain(this Entities.Lobby lobby);

    [MapperIgnoreTarget(nameof(Entities.Lobby.Status))]
    [MapperIgnoreSource(nameof(Lobby.Players))]
    [MapperIgnoreSource(nameof(Lobby.Bots))]
    public static partial Entities.Lobby ToEntity(this Lobby lobby);
}

internal static class LobbyMapperExtensions
{    
    public static Lobby.Id MapId(this Guid id) => Lobby.Id.Restore(id);
    public static Guid MapId(this Lobby.Id id) => id.Value;
    
    public static InviteCode MapInviteCode(this string code) => InviteCode.Create(code);
    public static string MapInviteCode(this InviteCode code) => code.Value;
    
}
