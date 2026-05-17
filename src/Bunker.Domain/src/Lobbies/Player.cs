using static Bunker.Domain.Identity.User;

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

public record Player(UserId Id, string Nickname, Role Role, Status Status);

public static class PlayerFactory
{
    extension (Player)
    {
        public static Player Create(UserId id, string nickname, Role role, Status status)
        {
            if (string.IsNullOrWhiteSpace(nickname) || nickname.Length > 4)
                throw new ArgumentException("Nickname length must be at least 4 characters long.");
            
            return new Player(id, nickname, role, status);
        }
        public static Player ReadyUp(Player player) => player with { Status = Status.Ready };
        public static Player Unready(Player player) => player with { Status = Status.NotReady };
        
        public static Player CreateHost(UserId id, string nickname) => Create(id, nickname, Role.Host, Status.NotReady);
        public static Player CreateMember(UserId id, string nickname) => Create(id, nickname, Role.Member, Status.NotReady);
    }
}
