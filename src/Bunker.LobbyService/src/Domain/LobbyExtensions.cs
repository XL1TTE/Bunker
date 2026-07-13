using Shared.Monads.Result;

namespace Bunker.LobbyService.Domain;

public static partial class LobbyExtensions
{
    extension (Lobby lobby)
    {
        // The host owns the Start button and has no readiness toggle, so they're
        // exempt from the all-ready requirement — only the other members must be ready.
        public bool AllReady => lobby.Players.Where(p => p.Role is not Host).All(p => p.Status is Status.Ready);

        public Result<Lobby, LobbyErrors.AddPlayerError> AddPlayer(PlayerParticipant player)
        {
            if (lobby.Players.Any(p => Equals(p.UserId, player.UserId)))
                return Result<Lobby, LobbyErrors.AddPlayerError>.Failure(new LobbyErrors.AddPlayerError.PlayerAlreadyInLobby());

            if (lobby.Participants.Count >= lobby.Capacity)
                return Result<Lobby, LobbyErrors.AddPlayerError>.Failure(new LobbyErrors.AddPlayerError.LobbyIsFull());

            lobby.Participants.Add(player);
            return Result<Lobby, LobbyErrors.AddPlayerError>.Success(lobby);
        }

        public Result<Lobby, LobbyErrors.AddPlayerError> AddBot(BotParticipant bot)
        {
            if (lobby.Participants.Count >= lobby.Capacity)
                return Result<Lobby, LobbyErrors.AddPlayerError>.Failure(new LobbyErrors.AddPlayerError.LobbyIsFull());

            lobby.Participants.Add(bot);
            return Result<Lobby, LobbyErrors.AddPlayerError>.Success(lobby);
        }

        public void AddCardPack(CardPackId packId)
        {
            lobby.Packs.Add(new LobbyCardPack(packId, lobby.PublicId));
        }

        public Result<Lobby, LobbyErrors.UpdateSettingsError> UpdateConfiguration(
            int capacity,
            bool isVisible,
            string? password,
            IEnumerable<LobbyCardPack> packs)
        {
            if (capacity < lobby.Participants.Count)
                return Result<Lobby, LobbyErrors.UpdateSettingsError>.Failure(new LobbyErrors.UpdateSettingsError.CapacityTooSmall());

            lobby.Capacity = capacity;
            lobby.PrivacyPolicy = new PrivacyPolicy(isVisible, password);
            lobby.Packs.Clear();
            foreach (var p in packs) lobby.Packs.Add(p);

            return Result<Lobby, LobbyErrors.UpdateSettingsError>.Success(lobby);
        }


        public Result<Lobby, LobbyErrors.ToggleReadyError> ToggleReady(AccountId playerId)
        {
            var player = lobby.Players.FirstOrDefault(p => p.UserId == playerId);
            if (player == null)
                return Result<Lobby, LobbyErrors.ToggleReadyError>.Failure(new LobbyErrors.ToggleReadyError.PlayerNotInLobby());

            player.Status = player.Status is Status.Ready ? Status.GetNotReady() : Status.GetReady();

            return Result<Lobby, LobbyErrors.ToggleReadyError>.Success(lobby);
        }

        public Lobby SetVisibility(PrivacyPolicy visibility)
        {
            lobby.PrivacyPolicy = visibility;
            return lobby;
        }

        public Result<Lobby, LobbyErrors.StartGameError> StartGame(AccountId callerId)
        {
            var host = lobby.Players.FirstOrDefault(p => p.Role is Host);
            if (host is null || host.UserId != callerId)
                return Result<Lobby, LobbyErrors.StartGameError>.Failure(new LobbyErrors.StartGameError.NotHost());

            if (lobby.AllReady is false)
                return Result<Lobby, LobbyErrors.StartGameError>.Failure(new LobbyErrors.StartGameError.NotAllReady());

            if (lobby.Players.Count() < 2)
                return Result<Lobby, LobbyErrors.StartGameError>.Failure(new LobbyErrors.StartGameError.InsufficientPlayers());

            lobby.State = LobbyState.Starting;
            return Result<Lobby, LobbyErrors.StartGameError>.Success(lobby);
        }

        public Result<PlayerParticipant, LobbyErrors.LeaveError> Leave(AccountId callerId)
        {
            var player = lobby.Players.FirstOrDefault(p => p.UserId == callerId);

            if (player is null)
                return Result<PlayerParticipant, LobbyErrors.LeaveError>.Failure(new LobbyErrors.LeaveError.PlayerNotInLobby());

            lobby.Participants.Remove(player);

            return Result<PlayerParticipant, LobbyErrors.LeaveError>.Success(player);
        }


        public Result<Lobby, LobbyErrors.KickError> Kick(LobbyParticipant.Id participantId, AccountId callerId)
        {
            var caller = lobby.Players.FirstOrDefault(p => p.UserId == callerId);

            if (caller is null || caller.Role is not Host)
                return Result<Lobby, LobbyErrors.KickError>.Failure(new LobbyErrors.KickError.KickerNotHost());

            var target = lobby.Participants.FirstOrDefault(p => p.PublicId == participantId);
            if (target is null)
                return Result<Lobby, LobbyErrors.KickError>.Failure(new LobbyErrors.KickError.KickParticipantNotFound());

            if (target.Role is Host)
                return Result<Lobby, LobbyErrors.KickError>.Failure(new LobbyErrors.KickError.CannotKickHost());

            lobby.Participants.Remove(target);
            return Result<Lobby, LobbyErrors.KickError>.Success(lobby);
        }

        public Result<Lobby, LobbyErrors.RemoveBotError> RemoveBot(LobbyParticipant.Id participantId, AccountId callerId)
        {
            var caller = lobby.Players.FirstOrDefault(p => p.UserId == callerId);
            if (caller is null || caller.Role is not Host)
                return Result<Lobby, LobbyErrors.RemoveBotError>.Failure(new LobbyErrors.RemoveBotError.BotRemoverNotHost());

            var bot = lobby.Bots.FirstOrDefault(b => b.PublicId == participantId);
            if (bot is null)
                return Result<Lobby, LobbyErrors.RemoveBotError>.Failure(new LobbyErrors.RemoveBotError.BotNotFound());

            lobby.Participants.Remove(bot);
            return Result<Lobby, LobbyErrors.RemoveBotError>.Success(lobby);
        }

        public void MarkInGame()
        {
            lobby.State = LobbyState.InGame;
        }

        public void ReopenAfterGame()
        {
            lobby.State = LobbyState.WaitingForPlayers;

            foreach (var player in lobby.Players)
            {
                player.Status = Status.GetNotReady();
            }
        }

        public void RevertStarting()
        {
            lobby.State = LobbyState.WaitingForPlayers;
        }
    }
}
