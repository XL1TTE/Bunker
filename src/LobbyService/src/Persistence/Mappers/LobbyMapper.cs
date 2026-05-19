using Bunker.LobbyService.Domain;
using Riok.Mapperly.Abstractions;

namespace Bunker.LobbyService.Persistence.Entities;

[Mapper]
[UseStaticMapper(typeof(LobbyParticipantMapper))]
[UseStaticMapper(typeof(LobbyMapperExtensions))]
[UseStaticMapper(typeof(PrivacyPolicyMapper))]
public static partial class LobbyMapper
{
    [MapperIgnoreSource(nameof(Lobby.Status))]
    public static partial Domain.Lobby ToDomain(this Lobby lobby);

    [MapperIgnoreTarget(nameof(Lobby.Status))]
    [MapperIgnoreSource(nameof(Domain.Lobby.Players))]
    [MapperIgnoreSource(nameof(Domain.Lobby.Bots))]
    public static partial Lobby ToEntity(this Domain.Lobby lobby);
}

internal static class LobbyMapperExtensions
{    
    public static Domain.Lobby.Id MapId(this Guid id) => Domain.Lobby.Id.Restore(id);
    public static Guid MapId(this Domain.Lobby.Id id) => id.Value;
    
    public static CardPackId MapCardPackId(this Guid id) => CardPackId.Create(id);
    public static Guid MapCardPackId(this CardPackId id) => id.Value;
    
    public static InviteCode MapInviteCode(this string code) => InviteCode.Create(code);
    public static string MapInviteCode(this InviteCode code) => code.Value;
    
}
