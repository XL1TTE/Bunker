using Microsoft.CodeAnalysis;

namespace Bunker.LobbyService.Transfers;

public abstract partial class Transfer
{
    public readonly record struct LobbyParticipant(
      string Id,
      string Nickname,
      string Role, // Host | Member
      string Status, // Ready | NotReady
      string Type, // Bot | Player
      
      string? AccountId,
      string? BotPresetId 
    );

    public readonly record struct LobbySnapshot
    (
        string Id,
        string InviteCode,
        int Capacity,
        bool Visible,
        string HostId,
        LobbyParticipant[] Participants,
        string[] CardPackIds  
    );
}
