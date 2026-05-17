namespace Bunker.Infrastructure.Identity;

public interface IUserIdentityContext
{
    Guid? UserId { get; }
    string? Nickname { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }

    void SetUser(Guid userId, string nickname, string? email);
}

internal class UserIdentityContext : IUserIdentityContext
{
    public Guid? UserId { get; private set; }
    public string? Nickname { get; private set; }
    public string? Email { get; private set; }

    public bool IsAuthenticated => UserId != null;

    public void SetUser(Guid userId, string nickname, string? email)
    {
        UserId = userId;
        Nickname = nickname;
        Email = email;
    }
}
