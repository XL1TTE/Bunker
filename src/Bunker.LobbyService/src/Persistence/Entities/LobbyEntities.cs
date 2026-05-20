using System.Collections.ObjectModel;

namespace Bunker.LobbyService.Persistence.Entities;

public class Lobby
{
    public required Guid PublicId { get; set; }
    public required string InviteCode { get; set; }
    public required int Capacity { get; set; }
    public required PrivacyPolicy PrivacyPolicy { get; set; }
    public string? Status { get; set; } // "WaitingForPlayers", "InGame", "Finished", etc.
    public Collection<LobbyParticipant> Participants { get; set; } = [];
    public Collection<LobbyCardPack> Packs { get; set; } = [];
}

public record PrivacyPolicy
{
    public required bool IsVisible { get; set; }
    public string? Password {get; set; }
}

public class LobbyParticipant
{
    public required Guid PublicId { get; set; }
    public required Guid LobbyId { get; set; }
    public required string Nickname { get; set; }
    public required string Role { get; set; } // "Host", "Member", etc.
    public required string Status { get; set; } // "Ready, "NotReady", etc.
}

public class PlayerParticipant : LobbyParticipant
{
    public required Guid UserId { get; set; }
}

public class BotParticipant : LobbyParticipant
{
    public required Guid PersonalityPresetId { get; set; } // For bots, reference to their personality preset
}

public class LobbyCardPack
{
    public required Guid LobbyId { get; set; }
    public required Guid PackId { get; set; }
}
