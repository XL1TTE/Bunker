using Bunker.Domain.Bots;
using Bunker.Domain.Cards;
using Bunker.Domain.Identity;
using Bunker.Domain.Lobbies;
using LobbyService.Persistence.Entities;

namespace LobbyService.Persistence.Mappers;

internal static class LobbyMappingHelpers
{
    public static Guid MapLobbyIdToGuid(LobbyId id) => id.Value;
    public static string MapInviteCodeToString(InviteCode code) => code.Value;
    public static Guid MapPlayerIdToGuid(PlayerId id) => id.Value;
    public static string MapPackIdToString(CardPackId id) => id.Value;
    public static int MapRoleToInt(ParticipantRole role) => (int)role;
    public static bool MapIsPublic(Lobby domain) => domain is PublicLobby;

    // Entity -> Domain
    public static LobbyId MapGuidToLobbyId(Guid guid) => LobbyIdFactory.Create(guid);
    public static InviteCode MapStringToInviteCode(string value) => InviteCodeFactory.Create(value);
    public static PlayerId MapGuidToPlayerId(Guid value) => PlayerIdFactory.Create(value);
    public static CardPackId MapStringToPackId(string value) => CardPackIdFactory.Create(value);
    public static ParticipantRole MapIntToRole(int value) => (ParticipantRole)value;

    // Nested Objects (Simple)
    public static Bot MapBotEntityToDomain(BotEntity entity) => BotFactory.Create(entity.Name, entity.PersonalityPreset);
    public static BotEntity MapBotDomainToEntity(Bot domain) => new() { Name = domain.Name, PersonalityPreset = domain.PersonalityPreset };
    
}
