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
                AccountId: participant is PlayerParticipant player ? player.UserId.Value.ToString() : null,
                BotPresetId: participant is BotParticipant bot ? bot.PersonalityPresetId.Value.ToString() : null
            );
    }
}

public static class LobbyMappers
{
    extension(Lobby lobby)
    {
        public Transfer.LobbySnapshot ToTransfer()
            => new(
                Id: lobby.PublicId.Value.ToString(),
                InviteCode: lobby.InviteCode.Value,
                Capacity: lobby.Capacity,
                Visible: lobby.PrivacyPolicy.IsVisible,
                HostId: lobby.Players.First(x => x.Role == Role.Host).UserId.Value.ToString(),
                Participants: lobby.Participants.Select(x => x.ToTransfer()).ToArray(),
                CardPackIds: lobby.Packs.Select(x => x.PackId.Value.ToString()).ToArray()
            );


    }
}
