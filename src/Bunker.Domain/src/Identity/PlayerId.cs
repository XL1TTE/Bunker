namespace Bunker.Domain.Identity;

public record PlayerId(string Value);

public static class PlayerIdFactory
{
    extension(PlayerId)
    {
        public static PlayerId Restore(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Player ID cannot be empty", nameof(value));
                
            return new PlayerId(value);
        }

        public static PlayerId Create()
        {
            return new PlayerId(Guid.NewGuid().ToString());
        }
    }
}
