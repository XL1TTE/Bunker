namespace Bunker.ContentService.Domain;

/// <summary>
/// Canned fallback Bunker Card content. Carries the three fields that set the stage for
/// a game: the Catastrophe that befell the outside world, the Survival Duration players
/// must justify surviving, and the Bunker Environment (the usefulness lens players argue
/// their attribute cards against). Used as compensation content when AI generation fails.
/// </summary>
/// <param name="PublicId">The unique public identifier for the bunker card.</param>
/// <param name="Catastrophe">What happened to the outside world.</param>
/// <param name="SurvivalDuration">The time horizon players must justify surviving (e.g. "10 years").</param>
/// <param name="BunkerEnvironment">Free-prose description of what the bunker has and lacks.</param>
public record BunkerCard(BunkerCard.Id PublicId, string Catastrophe, string SurvivalDuration, string BunkerEnvironment)
{
    public readonly record struct Id(Guid Value)
    {
        public static Id New() => new(Guid.NewGuid());
        public static Id Create(Guid value) => new(value);
    }

    public string Catastrophe { get; internal set; } =
        string.IsNullOrWhiteSpace(Catastrophe) | Catastrophe.Length < 8
        ? throw new ArgumentException("Bunker card catastrophe must be at least 8 characters long.")
        : Catastrophe;

    public string SurvivalDuration { get; internal set; } =
        string.IsNullOrWhiteSpace(SurvivalDuration) | SurvivalDuration.Length < 3
        ? throw new ArgumentException("Bunker card survival duration must be at least 3 characters long.")
        : SurvivalDuration;

    public string BunkerEnvironment { get; internal set; } =
        string.IsNullOrWhiteSpace(BunkerEnvironment) | BunkerEnvironment.Length < 10
        ? throw new ArgumentException("Bunker card bunker environment must be at least 10 characters long.")
        : BunkerEnvironment;
}

public static class BunkerCardFactory
{
    extension(BunkerCard)
    {
        public static BunkerCard New(string catastrophe, string survivalDuration, string bunkerEnvironment)
            => new BunkerCard(BunkerCard.Id.New(), catastrophe, survivalDuration, bunkerEnvironment);
        public static BunkerCard Create(Guid id, string catastrophe, string survivalDuration, string bunkerEnvironment)
            => new BunkerCard(BunkerCard.Id.Create(id), catastrophe, survivalDuration, bunkerEnvironment);
    }
}

public static class BunkerCardExtensions
{
    extension(BunkerCard card)
    {
        public void UpdateCatastrophe(string catastrophe)
        {
            if (string.IsNullOrWhiteSpace(catastrophe) || catastrophe.Length < 8)
                throw new ArgumentException("Bunker card catastrophe must be at least 8 characters long.");

            card.Catastrophe = catastrophe;
        }

        public void UpdateSurvivalDuration(string survivalDuration)
        {
            if (string.IsNullOrWhiteSpace(survivalDuration) || survivalDuration.Length < 3)
                throw new ArgumentException("Bunker card survival duration must be at least 3 characters long.");

            card.SurvivalDuration = survivalDuration;
        }

        public void UpdateBunkerEnvironment(string bunkerEnvironment)
        {
            if (string.IsNullOrWhiteSpace(bunkerEnvironment) || bunkerEnvironment.Length < 10)
                throw new ArgumentException("Bunker card bunker environment must be at least 10 characters long.");

            card.BunkerEnvironment = bunkerEnvironment;
        }
    }
}