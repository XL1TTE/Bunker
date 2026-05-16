namespace Bunker.Infrastructure.Identity;

public interface IUserIdentityContext
{
    string? UserId { get; }
    string? Nickname { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    void SetUser(string userId, string nickname, string? email);
}

internal class UserIdentityContext : IUserIdentityContext
{
    public string? UserId { get; private set; }
    public string? Nickname { get; private set; }
    public string? Email { get; private set; }
    public bool IsAuthenticated => UserId != null;

    public void SetUser(string userId, string nickname, string? email)
    {
        UserId = userId;
        Nickname = nickname;
        Email = email;
    }
}
