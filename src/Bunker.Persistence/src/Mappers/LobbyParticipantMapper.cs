using Bunker.Domain.Bots;
using Bunker.Domain.Identity;
using Bunker.Domain.Lobbies;
using Bunker.Persistence.Mappers;
using Riok.Mapperly.Abstractions;

namespace LobbyService.Persistence.Mappers;

[Mapper]
[UseStaticMapper(typeof(LobbyParticipantExtensions))]
[UseStaticMapper(typeof(BotPersonalityPresetMapper))]
public static partial class LobbyParticipantMapper
{
    [MapDerivedType<PlayerParticipant, Entities.PlayerParticipant>]
    [MapDerivedType<BotParticipant, Entities.BotParticipant>]
    public static partial Entities.LobbyParticipant ToEntity(this LobbyParticipant participant);

    [MapDerivedType<Entities.PlayerParticipant, PlayerParticipant>]
    [MapDerivedType<Entities.BotParticipant, BotParticipant>]
    public static partial LobbyParticipant ToDomain(this Entities.LobbyParticipant participant);
}

internal static class LobbyParticipantExtensions
{
    public static PersonalityPreset.Id MapPersonalityPresetId(this Guid id) => PersonalityPreset.Id.Create(id);
    public static Guid MapPersonalityPresetId(this PersonalityPreset.Id id) => id.Value;
    
    public static Guid MapUserId(this User.Id id) => id.Value;
    public static User.Id MapUserId(this Guid id) => User.Id.Restore(id);
    
    public static Guid MapLobbyId(this Lobby.Id id) => id.Value;
    public static Lobby.Id MapLobbyId(this Guid id) => Lobby.Id.Restore(id);
    
    public static Guid MapId(this LobbyParticipant.Id id) => id.Value;
    public static LobbyParticipant.Id MapId(this Guid id) => LobbyParticipant.Id.Restore(id);
    
    public static Status MapStatus(this string status) => status switch
    {
        "Ready" => Status.Ready,
        "NotReady" => Status.NotReady,
        _ => throw new ArgumentException("Invalid status value.")
    };
    public static string MapStatus(this Status status) => status switch
    {
        Ready => "Ready",
        NotReady => "NotReady",
        _ => throw new ArgumentException("Invalid status value.")
    };
    
    public static string MapRole(this Role role) => role switch
    {
        Host => "Host",
        Member => "Member",
        _ => throw new ArgumentException("Invalid role value.")
    };
    public static Role MapRole(this string role) => role switch
    {
        "Host" => Role.Host,
        "Member" => Role.Member,
        _ => throw new ArgumentException("Invalid role value.")
    };
}
