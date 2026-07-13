using Bunker.LobbyService.Domain;

namespace Bunker.LobbyService.Transfers;

public static class LobbyParticipantMappers
{
    extension(LobbyParticipant participant)
    {
        public Transfer.LobbyParticipant ToTransfer()
            => new(
                Id: participant.PublicId.Value.ToString(),
                Nickname: participant.Nickname,
                Role: participant.Role.ToString(),
                Status: participant.Status.ToString(),
                Type: participant is BotParticipant ? "Bot" : "Player",
                AccountId: participant is Player player ? player.UserId.Value.ToString() : null,
                PersonalityPresetId: participant is BotParticipant bot ? bot.PersonalityPresetId.Value.ToString() : null
            );
    }
}

public static class LobbyMappers
{
    extension(Lobby lobby)
    {
        public Transfer.LobbySnapshot ToTransfer()
        {
            var host = lobby.Participants.FirstOrDefault(x => x.Role == Role.Host);
            return new Transfer.LobbySnapshot(
                Id: lobby.PublicId.Value.ToString(),
                Name: lobby.Name.Value,
                Capacity: lobby.Capacity,
                IsPublic: lobby.PrivacyPolicy.IsVisible,
                HostParticipantId: host?.PublicId.Value.ToString(),
                State: lobby.State.ToString(),
                Participants: lobby.Participants.Select(x => x.ToTransfer()).ToArray(),
                SelectedPackIds: lobby.Packs.Select(x => x.PackId.Value.ToString()).ToArray()
            );
        }

        public Transfer.LobbySummary ToSummary()
        {
            var host = lobby.Participants.FirstOrDefault(x => x.Role == Role.Host);
            return new Transfer.LobbySummary(
                Id: lobby.PublicId.Value.ToString(),
                Name: lobby.Name.Value,
                Capacity: lobby.Capacity,
                CurrentPlayers: lobby.Participants.Count,
                HasPassword: !string.IsNullOrEmpty(lobby.PrivacyPolicy.Password),
                HostNickname: host?.Nickname ?? string.Empty,
                SelectedPackIds: lobby.Packs.Select(x => x.PackId.Value.ToString()).ToArray()
            );
        }
    }
}
