using Bunker.Domain.Cards;
using Bunker.Domain.Lobbies;
using LobbyService.Persistence.Entities;

namespace LobbyService.Persistence.Mappers;

internal static class CardPackMapper
{
    public static LobbyCardPackEntity ToEntity(CardPackId domain)
    {
        return new LobbyCardPackEntity
        {
            PackId = domain.Value
        };
    }
    public static CardPackId ToDomain(LobbyCardPackEntity entity)
    {
        return CardPackIdFactory.Create(entity.PackId);
    }
}
