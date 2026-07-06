using Bunker.LobbyService.Domain;
using Riok.Mapperly.Abstractions;

namespace Bunker.LobbyService.Persistence.Entities;

[Mapper]
[UseStaticMapper(typeof(LobbyParticipantMapper))]
[UseStaticMapper(typeof(LobbyMapperExtensions))]
[UseStaticMapper(typeof(PrivacyPolicyMapper))]
public static partial class LobbyMapper
{
    [MapProperty(nameof(Lobby.Status), nameof(Domain.Lobby.State))]
    public static partial Domain.Lobby ToDomain(this Lobby lobby);

    [MapProperty(nameof(Domain.Lobby.State), nameof(Lobby.Status))]
    [MapperIgnoreSource(nameof(Domain.Lobby.Players))]
    [MapperIgnoreSource(nameof(Domain.Lobby.Bots))]
    public static partial Lobby ToEntity(this Domain.Lobby lobby);

    [MapProperty(nameof(Domain.Lobby.State), nameof(Lobby.Status))]
    [MapperIgnoreSource(nameof(Domain.Lobby.Players))]
    [MapperIgnoreSource(nameof(Domain.Lobby.Bots))]
    public static partial void ApplyUpdate([MappingTarget] this Lobby entity, Domain.Lobby lobby);

}

internal static class LobbyMapperExtensions
{
    public static Domain.Lobby.Id MapId(this Guid id) => Domain.Lobby.Id.Restore(id);
    public static Guid MapId(this Domain.Lobby.Id id) => id.Value;

    public static CardPackId MapCardPackId(this Guid id) => CardPackId.Create(id);
    public static Guid MapCardPackId(this CardPackId id) => id.Value;

    public static InviteCode MapInviteCode(this string code) => InviteCode.Create(code);
    public static string MapInviteCode(this InviteCode code) => code.Value;

    public static Domain.LobbyState MapState(this string? state) => state switch
    {
        "WaitingForPlayers" => Domain.LobbyState.WaitingForPlayers,
        "Starting" => Domain.LobbyState.Starting,
        "InGame" => Domain.LobbyState.InGame,
        null => Domain.LobbyState.WaitingForPlayers,
        _ => throw new ArgumentException("Invalid lobby state value.")
    };

    public static string MapState(this Domain.LobbyState state) => state switch
    {
        Domain.LobbyState.WaitingForPlayers => "WaitingForPlayers",
        Domain.LobbyState.Starting => "Starting",
        Domain.LobbyState.InGame => "InGame",
        _ => throw new ArgumentException("Invalid lobby state value.")
    };
}