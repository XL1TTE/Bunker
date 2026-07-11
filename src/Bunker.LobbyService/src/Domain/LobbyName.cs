namespace Bunker.LobbyService.Domain;

public readonly record struct LobbyName(string Value);

public static class LobbyNameFactory
{
    private const int MaxLength = 64;

    extension (LobbyName)
    {
        public static LobbyName Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Lobby name must not be empty.");

            var trimmed = value.Trim();
            if (trimmed.Length > MaxLength)
                throw new ArgumentException($"Lobby name must be at most {MaxLength} characters.");

            return new LobbyName(trimmed);
        }
    }
}