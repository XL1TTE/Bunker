using Bunker.LobbyService.Domain;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Bunker.LobbyService.Persistence.Conversions;

public static class ValueConversions
{
    public static ValueConverter<Lobby.Id, Guid> LobbyIdConverter { get; } =
        new(v => v.Value, v => Lobby.Id.Restore(v));
    public static ValueComparer<Lobby.Id> LobbyIdComparer { get; } =
        new((a, b) => a.Equals(b), v => v.GetHashCode(), v => v);

    public static ValueConverter<LobbyParticipant.Id, Guid> ParticipantIdConverter { get; } =
        new(v => v.Value, v => LobbyParticipant.Id.Restore(v));
    public static ValueComparer<LobbyParticipant.Id> ParticipantIdComparer { get; } =
        new((a, b) => a.Equals(b), v => v.GetHashCode(), v => v);

    public static ValueConverter<AccountId, string> AccountIdConverter { get; } =
        new(v => v.Value, v => AccountId.Create(v));
    public static ValueComparer<AccountId> AccountIdComparer { get; } =
        new((a, b) => a.Equals(b), v => v.GetHashCode(), v => v);

    public static ValueConverter<CardPackId, Guid> CardPackIdConverter { get; } =
        new(v => v.Value, v => CardPackId.Create(v));
    public static ValueComparer<CardPackId> CardPackIdComparer { get; } =
        new((a, b) => a.Equals(b), v => v.GetHashCode(), v => v);

    public static ValueConverter<BotPersonalityId, Guid> BotPersonalityIdConverter { get; } =
        new(v => v.Value, v => BotPersonalityId.Create(v));
    public static ValueComparer<BotPersonalityId> BotPersonalityIdComparer { get; } =
        new((a, b) => a.Equals(b), v => v.GetHashCode(), v => v);

    public static ValueConverter<InviteCode, string> InviteCodeConverter { get; } =
        new(v => v.Value, v => new InviteCode(v));
    public static ValueComparer<InviteCode> InviteCodeComparer { get; } =
        new((a, b) => a.Equals(b), v => v.GetHashCode(), v => v);

    public static ValueConverter<LobbyName, string> LobbyNameConverter { get; } =
        new(v => v.Value, v => new LobbyName(v));
    public static ValueComparer<LobbyName> LobbyNameComparer { get; } =
        new((a, b) => a.Equals(b), v => v.GetHashCode(), v => v);

    public static ValueConverter<Role, string> RoleConverter { get; } =
        new(r => r.GetType().Name, v => v == "Host" ? Role.Host : Role.Member);
    public static ValueComparer<Role> RoleComparer { get; } =
        new((a, b) => a.Equals(b), v => v.GetHashCode(), v => v);

    public static ValueConverter<Status, string> StatusConverter { get; } =
        new(s => s.GetType().Name, v => v == "Ready" ? Status.GetReady() : Status.GetNotReady());
    public static ValueComparer<Status> StatusComparer { get; } =
        new((a, b) => a.Equals(b), v => v.GetHashCode(), v => v);

    public static ValueConverter<LobbyState, string> LobbyStateConverter { get; } =
        new(v => v.ToString(), v => v == null ? LobbyState.WaitingForPlayers : Enum.Parse<LobbyState>(v));    
    public static ValueComparer<LobbyState> LobbyStateComparer { get; } =
        new((a, b) => a.Equals(b), v => v.GetHashCode(), v => v);
}
