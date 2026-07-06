using Shared.Monads.Result;

namespace Bunker.LobbyService.Domain;

public readonly record struct LeaveOutcome(bool Destroyed);

public static partial class LobbyExtensions
{
    extension (Lobby lobby)
    {
        public bool AllReady => lobby.Players.All(p => p.Status is Status.Ready);

        public Result<Lobby, LobbyErrors.AddPlayerError> AddPlayer(PlayerParticipant player)
        {
            if (lobby.Players.Any(p => Equals(p.UserId, player.UserId)))
                return Result<Lobby, LobbyErrors.AddPlayerError>.Failure(new LobbyErrors.PlayerAlreadyInLobby());

            if (lobby.Participants.Count >= lobby.Capacity)
                return Result<Lobby, LobbyErrors.AddPlayerError>.Failure(new LobbyErrors.LobbyIsFull());

            lobby.Participants.Add(player);
            return Result<Lobby, LobbyErrors.AddPlayerError>.Success(lobby);
        }

        public Result<Lobby, LobbyErrors.AddPlayerError> AddBot(BotParticipant bot)
        {
            if (lobby.Participants.Count >= lobby.Capacity)
                return Result<Lobby, LobbyErrors.AddPlayerError>.Failure(new LobbyErrors.LobbyIsFull());

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
                return Result<Lobby, LobbyErrors.UpdateSettingsError>.Failure(new LobbyErrors.CapacityTooSmall());

            lobby.Capacity = capacity;
            lobby.PrivacyPolicy = new PrivacyPolicy(isVisible, password);
            lobby.Packs.Clear();
            foreach (var p in packs) lobby.Packs.Add(p);

            return Result<Lobby, LobbyErrors.UpdateSettingsError>.Success(lobby);
        }

        public Result<Lobby, LobbyErrors.RemovePlayerError> RemovePlayer(AccountId playerId)
        {
            var player = lobby.Players.FirstOrDefault(p => Equals(p.UserId, playerId));
            if (player == null)
                return Result<Lobby, LobbyErrors.RemovePlayerError>.Failure(new LobbyErrors.PlayerNotFound());

            lobby.Participants.Remove(player);

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

        public Result<Lobby, LobbyErrors.ToggleReadyError> ToggleReady(AccountId playerId)
        {
            var player = lobby.Players.FirstOrDefault(p => p.UserId == playerId);
            if (player == null)
                return Result<Lobby, LobbyErrors.ToggleReadyError>.Failure(new LobbyErrors.PlayerNotInLobby());

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
                return Result<Lobby, LobbyErrors.StartGameError>.Failure(new LobbyErrors.NotHost());

            if (lobby.AllReady is false)
                return Result<Lobby, LobbyErrors.StartGameError>.Failure(new LobbyErrors.NotAllReady());

            if (lobby.Players.Count() < 2)
                return Result<Lobby, LobbyErrors.StartGameError>.Failure(new LobbyErrors.InsufficientPlayers());

            lobby.State = LobbyState.Starting;
            return Result<Lobby, LobbyErrors.StartGameError>.Success(lobby);
        }

        public Result<LeaveOutcome, LobbyErrors.LeaveError> Leave(AccountId callerId)
        {
            var player = lobby.Players.FirstOrDefault(p => p.UserId == callerId);
            if (player is null)
                return Result<LeaveOutcome, LobbyErrors.LeaveError>.Failure(new LobbyErrors.CallerNotInLobby());

            if (player.Role is Host)
                return Result<LeaveOutcome, LobbyErrors.LeaveError>.Success(new LeaveOutcome(Destroyed: true));

            lobby.Participants.Remove(player);
            return Result<LeaveOutcome, LobbyErrors.LeaveError>.Success(new LeaveOutcome(Destroyed: false));
        }

        public Result<Lobby, LobbyErrors.KickError> Kick(LobbyParticipant.Id participantId, AccountId callerId)
        {
            var caller = lobby.Players.FirstOrDefault(p => p.UserId == callerId);
            if (caller is null || caller.Role is not Host)
                return Result<Lobby, LobbyErrors.KickError>.Failure(new LobbyErrors.KickerNotHost());

            var target = lobby.Participants.FirstOrDefault(p => p.PublicId == participantId);
            if (target is null)
                return Result<Lobby, LobbyErrors.KickError>.Failure(new LobbyErrors.KickParticipantNotFound());

            if (target.Role is Host)
                return Result<Lobby, LobbyErrors.KickError>.Failure(new LobbyErrors.CannotKickHost());

            lobby.Participants.Remove(target);
            return Result<Lobby, LobbyErrors.KickError>.Success(lobby);
        }

        public Result<Lobby, LobbyErrors.RemoveBotError> RemoveBot(LobbyParticipant.Id participantId, AccountId callerId)
        {
            var caller = lobby.Players.FirstOrDefault(p => p.UserId == callerId);
            if (caller is null || caller.Role is not Host)
                return Result<Lobby, LobbyErrors.RemoveBotError>.Failure(new LobbyErrors.BotRemoverNotHost());

            var bot = lobby.Bots.FirstOrDefault(b => b.PublicId == participantId);
            if (bot is null)
                return Result<Lobby, LobbyErrors.RemoveBotError>.Failure(new LobbyErrors.BotNotFound());

            lobby.Participants.Remove(bot);
            return Result<Lobby, LobbyErrors.RemoveBotError>.Success(lobby);
        }

        public void MarkInGame()
        {
            lobby.State = LobbyState.InGame;
        }

        public void RevertStarting()
        {
            lobby.State = LobbyState.WaitingForPlayers;
        }
    }
}