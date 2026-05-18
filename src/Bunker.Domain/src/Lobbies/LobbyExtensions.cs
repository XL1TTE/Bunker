using Bunker.Domain.Cards;
using Bunker.Domain.Identity;
using Shared.Monads.Result;

namespace Bunker.Domain.Lobbies;

public static partial class LobbyExtensions
{
    extension (Lobby lobby)
    {
        public bool AllReady => lobby.Players.All(p => p.Status is Ready);

        public Result<Lobby, LobbyErrors.AddPlayerError> AddPlayer(PlayerParticipant player)
        {
            if (lobby.Players.Any(p => Equals(p.UserId, player.UserId)))
                return Result<Lobby, LobbyErrors.AddPlayerError>.Failure(new LobbyErrors.PlayerAlreadyInLobby());

            if (lobby.Participants.Count >= lobby.Capacity)
                return Result<Lobby, LobbyErrors.AddPlayerError>.Failure(new LobbyErrors.LobbyIsFull());

            lobby.Participants.Add(player);
            return Result<Lobby, LobbyErrors.AddPlayerError>.Success(lobby);
        }

        public void AddBot(BotParticipant bot)
        {
            if (lobby.Participants.Count >= lobby.Capacity)
                throw new InvalidOperationException("Lobby is full");

            lobby.Participants.Add(bot);
        }

        public void AddCardPack(CardPack.Id packId)
        {
            lobby.Packs.Add(new LobbyCardPack(packId, lobby.PublicId));
        }

        public Result<Lobby, LobbyErrors.UpdateSettingsError> UpdateSettings(
            int capacity,
            IEnumerable<BotParticipant> bots,
            IEnumerable<LobbyCardPack> packs)
        {
            if (capacity < lobby.Players.Count())
                return Result<Lobby, LobbyErrors.UpdateSettingsError>.Failure(new LobbyErrors.CapacityTooSmall());

            lobby.Capacity = capacity;

            // Sync Card Packs
            lobby.Packs.Clear();
            foreach (var p in packs) lobby.Packs.Add(p);

            // Sync Bots (Keep humans, replace bots)
            var currentBots = lobby.Bots.ToList();
            foreach (var b in currentBots) lobby.Participants.Remove(b);
            foreach (var b in bots) lobby.Participants.Add(b);

            return Result<Lobby, LobbyErrors.UpdateSettingsError>.Success(lobby);
        }

        public Result<Lobby, LobbyErrors.RemovePlayerError> RemovePlayer(User.Id playerId)
        {
            var player = lobby.Players.FirstOrDefault(p => Equals(p.UserId, playerId));
            if (player == null)
                return Result<Lobby, LobbyErrors.RemovePlayerError>.Failure(new LobbyErrors.PlayerNotFound());

            lobby.Participants.Remove(player);

            // Handle Host Migration
            if (player.Role is Host)
            {
                var nextPlayer = lobby.Players.FirstOrDefault();
                if (nextPlayer != null)
                {
                    nextPlayer.Role = Role.Host;
                }
            }

            return Result<Lobby, LobbyErrors.RemovePlayerError>.Success(lobby);
        }

        public Result<Lobby, LobbyErrors.ToggleReadyError> ToggleReady(User.Id playerId)
        {
            var player = lobby.Players.FirstOrDefault(p => p.UserId == playerId);
            if (player == null)
                return Result<Lobby, LobbyErrors.ToggleReadyError>.Failure(new LobbyErrors.PlayerNotInLobby());

            player.Status = player.Status is Ready ? Status.NotReady : Status.Ready;

            return Result<Lobby, LobbyErrors.ToggleReadyError>.Success(lobby);
        }

        public Lobby SetVisibility(PrivacyPolicy visibility)
        {
            lobby.PrivacyPolicy = visibility;
            return lobby;
        }
    }
}
