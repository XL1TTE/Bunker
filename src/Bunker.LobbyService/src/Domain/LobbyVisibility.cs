namespace Bunker.LobbyService.Domain;

public record PrivacyPolicy(bool IsVisible, string? Password)
{
    public static PrivacyPolicy PublicPolicy(string? password = null) => new(true, password);
    public static PrivacyPolicy PrivatePolicy(string password) => new(false, password);
};
