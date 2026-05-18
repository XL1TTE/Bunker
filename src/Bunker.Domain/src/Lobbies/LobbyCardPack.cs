using Bunker.Domain.Cards;

namespace Bunker.Domain.Lobbies;

public record LobbyCardPack(CardPack.Id PackId, Lobby.Id LobbyId);
