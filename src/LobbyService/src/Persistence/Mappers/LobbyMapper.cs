using Bunker.Domain.Bots;
using Bunker.Domain.Cards;
using Bunker.Domain.Identity;
using Bunker.Domain.Lobbies;
using LobbyService.Persistence.Entities;
using Riok.Mapperly.Abstractions;

namespace LobbyService.Persistence.Mappers;

[Mapper]
[UseStaticMapper(typeof(LobbyMappingHelpers))]
[UseStaticMapper(typeof(CardPackMapper))]
static partial class LobbyMapper
{
    public static Lobby ToDomain(LobbyEntity entity) => entity.IsPublic ? MapToPublic(entity) : MapToPrivate(entity);
    public static partial LobbyEntity ToEntity(Lobby domain);


    [MapDerivedType<PublicLobby, LobbyEntity>]
    [MapDerivedType<PrivateLobby, LobbyEntity>]
    public static partial void ApplyUpdate([MappingTarget] this LobbyEntity entity, Lobby update);
    

    [MapperIgnoreSource(nameof(LobbyEntity.IsPublic))]
    private static partial PublicLobby MapToPublic(LobbyEntity entity);

    [MapperIgnoreSource(nameof(LobbyEntity.IsPublic))]
    private static partial PrivateLobby MapToPrivate(LobbyEntity entity);

    [MapperIgnoreTarget(nameof(LobbyParticipantEntity.Id))]
    [MapperIgnoreTarget(nameof(LobbyParticipantEntity.LobbyId))]
    [MapProperty(nameof(Player.Id), nameof(LobbyParticipantEntity.PlayerId))]
    private static partial LobbyParticipantEntity MapParticipantToEntity(Bunker.Domain.Lobbies.Player participant);

    [MapProperty(nameof(LobbyParticipantEntity.PlayerId), nameof(Player.Id))]
    [MapperIgnoreSource(nameof(LobbyParticipantEntity.LobbyId))]
    [MapperIgnoreSource(nameof(LobbyParticipantEntity.Id))]
    private static partial Bunker.Domain.Lobbies.Player MapParticipantToDomain(LobbyParticipantEntity entity);

    // private static void AfterMap(PublicLobby source, LobbyEntity target) => target.IsPublic = true;
    // private static void AfterMap(PrivateLobby source, LobbyEntity target) => target.IsPublic = false;
}
