using Microsoft.CodeAnalysis;

namespace Bunker.LobbyService.Transfers;

public abstract partial class Transfer
{
    public readonly record struct LobbyParticipant(
        string Id,
        string Nickname,
        string Role,
        string Status,
        string Type,
        string? AccountId,
        string? PersonalityPresetId
    );

    public readonly record struct LobbySnapshot(
        string Id,
        string Name,
        int Capacity,
        bool IsPublic,
        string? HostParticipantId,
        string State,
        LobbyParticipant[] Participants,
        string[] SelectedPackIds
    );

    public readonly record struct LobbySummary(
        string Id,
        string Name,
        int Capacity,
        int CurrentPlayers,
        bool HasPassword,
        string HostNickname,
        string[] SelectedPackIds
    );

    public readonly record struct ChatMessage(
        string Id,
        string ParticipantId,
        string Nickname,
        string Text,
        DateTime SentAt
    );

    public readonly record struct LobbyListResponse(
        LobbySummary[] Items,
        int Total
    );

    public readonly record struct LobbyInviteCode(
        string InviteCode
    );
}