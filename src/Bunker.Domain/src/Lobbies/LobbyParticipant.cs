using Bunker.Domain.Bots;
using Bunker.Domain.Identity;

namespace Bunker.Domain.Lobbies;

public abstract record Role
{
    public static Role Host   { get; } = new Host();
    public static Role Member { get; } = new Member();
}
public record Host   : Role;
public record Member : Role;

public abstract record Status
{
    public static Status Ready    { get; } = new Ready();
    public static Status NotReady { get; } = new NotReady();
}
public record Ready    : Status;
public record NotReady : Status;


/// <summary>
/// Owned by a lobby, represents either a player or a bot participant. Contains information about the participant's nickname, role, and ready status.
/// </summary>
public abstract record LobbyParticipant(LobbyParticipant.Id PublicId, Lobby.Id LobbyId, string Nickname, Role Role, Status Status)
{
    public readonly record struct Id(Guid Value)
    {
        public static Id Empty { get; } = new (Guid.Empty);
        public static Id New() => new (Guid.NewGuid());
        public static Id Restore(Guid value) => new (value);
    }
        
    public string Nickname { get; internal set; } = Nickname;
    public Role Role { get; internal set; } = Role;
    public Status Status { get; internal set; } = Status;
}
public record PlayerParticipant(LobbyParticipant.Id PublicId, User.Id UserId, Lobby.Id LobbyId, string Nickname, Role Role) 
    : LobbyParticipant(PublicId: PublicId, LobbyId: LobbyId, Nickname: Nickname, Role: Role, Status: Status.NotReady);
    
public record BotParticipant(LobbyParticipant.Id PublicId, Lobby.Id LobbyId, string Nickname, Role Role, PersonalityPreset.Id PersonalityPresetId) 
    : LobbyParticipant(PublicId: PublicId, LobbyId: LobbyId, Nickname: Nickname, Role: Role, Status: Status.Ready);
    

public static class PlayerParticipantFactory
{
    extension (PlayerParticipant)
    {
        public static PlayerParticipant New(User.Id userId, Lobby.Id lobbyId, string nickname, Role role)
        {
            if (string.IsNullOrWhiteSpace(nickname) || nickname.Length < 4)
                throw new ArgumentException("Nickname length must be at least 4 characters long.");
            
            return new PlayerParticipant(LobbyParticipant.Id.New(), userId, lobbyId, nickname, role);
        }
        
        public static PlayerParticipant Create(LobbyParticipant.Id id, User.Id userId, Lobby.Id lobbyId, string nickname, Role role, Status status)
        {
            if (string.IsNullOrWhiteSpace(nickname) || nickname.Length < 4)
                throw new ArgumentException("Nickname length must be at least 4 characters long.");
            
            return new PlayerParticipant(id, userId, lobbyId, nickname, role) { Status = status };
        }
    }
}

public static class PlayerParticipantExtensions
{
    public static PlayerParticipant ReadyUp(PlayerParticipant player)
    {
        player.Status = Status.Ready;
        return player;
    }
    public static PlayerParticipant Unready(PlayerParticipant player)
    {
        player.Status = Status.NotReady;
        return player;
    }
    
}
