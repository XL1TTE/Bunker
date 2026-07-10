namespace Bunker.ContentService.Domain;

public record HealthCard : Card
{
    public string Health { get; init; }

    internal HealthCard(Card.Id PublicId, string Health) : base(PublicId)
        => (this.PublicId, this.Health) = (PublicId, Health);

    public static HealthCard CreateNew(string Health) => CreateValid(Id.New(), Health);
    public static HealthCard Restore(Id Id, string Health) => CreateValid(Id, Health);

    private static HealthCard CreateValid(Id Id, string Health)
        => string.IsNullOrWhiteSpace(Health) | Health.Length < 8
        ? throw new ArgumentException("Health must be at least 8 characters long.")
        : new HealthCard(Id, Health);
}

public static class HealthCardExtensions
{
    extension(HealthCard card)
    {
        public HealthCard WithHealth(string Health) => card with {Health = Health};
    }
}