namespace Bunker.AccountService.Domain;

public record struct Nickname(string Value);

public static class NicknameFactory
{
    extension(Nickname)
    {
        public static Nickname Restore(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length < 3 || value.Length > 32)
                throw new ArgumentException("Nickname must be between 3 and 32 characters", nameof(value));
                
            return new Nickname(value);
        }
    }
}
