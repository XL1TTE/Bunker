using Riok.Mapperly.Abstractions;

namespace Bunker.LobbyService.Persistence.Entities;

[Mapper]
[UseStaticMapper(typeof(LobbyParticipantExtensions))]
public static partial class LobbyParticipantMapper
{
    [MapDerivedType<Domain.PlayerParticipant, PlayerParticipant>]
    [MapDerivedType<Domain.BotParticipant, BotParticipant>]
    public static partial LobbyParticipant ToEntity(this Domain.LobbyParticipant participant);

    [MapDerivedType<PlayerParticipant, Domain.PlayerParticipant>]
    [MapDerivedType<BotParticipant, Domain.BotParticipant>]
    public static partial Domain.LobbyParticipant ToDomain(this LobbyParticipant participant);
}

internal static class LobbyParticipantExtensions
{
    public static Domain.BotPersonalityId MapPersonalityPresetId(this Guid id) => Domain.BotPersonalityId.Create(id);
    public static Guid MapPersonalityPresetId(this Domain.BotPersonalityId id) => id.Value;
    
    public static Guid MapUserId(this Domain.AccountId id) => id.Value;
    public static Domain.AccountId MapUserId(this Guid id) => Domain.AccountId.Create(id);
    
    public static Guid MapLobbyId(this Domain.Lobby.Id id) => id.Value;
    public static Domain.Lobby.Id MapLobbyId(this Guid id) => Domain.Lobby.Id.Restore(id);
    
    public static Guid MapId(this Domain.LobbyParticipant.Id id) => id.Value;
    public static Domain.LobbyParticipant.Id MapId(this Guid id) => Domain.LobbyParticipant.Id.Restore(id);
    
    public static Domain.Status MapStatus(this string status) => status switch
    {
        "Ready" => Domain.Status.GetReady(),
        "NotReady" => Domain.Status.GetNotReady(),
        _ => throw new ArgumentException("Invalid status value.")
    };
    public static string MapStatus(this Domain.Status status) => status switch
    {
        Domain.Status.Ready => "Ready",
        Domain.Status.NotReady => "NotReady",
        _ => throw new ArgumentException("Invalid status value.")
    };
    
    public static string MapRole(this Domain.Role role) => role switch
    {
        Domain.Host => "Host",
        Domain.Member => "Member",
        _ => throw new ArgumentException("Invalid role value.")
    };
    public static Domain.Role MapRole(this string role) => role switch
    {
        "Host" => Domain.Role.Host,
        "Member" => Domain.Role.Member,
        _ => throw new ArgumentException("Invalid role value.")
    };
}
