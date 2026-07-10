namespace Bunker.GameService.Configuration;

/// <summary>
/// Durability-friendly timeouts that drive the automatic, turn-based game loop.
/// Defaults are inline so the service runs with no configuration; override via the
/// "Game" configuration section (e.g. shorter values for fast dev runs).
/// </summary>
public sealed class GameTimingOptions
{
    public TimeSpan BunkerIntroduction { get; set; } = TimeSpan.FromSeconds(15);

    public TimeSpan TurnTimeout { get; set; } = TimeSpan.FromSeconds(60);

    /// <summary>Duration of the free-for-all Discussion window before closing turns.</summary>
    public TimeSpan DiscussionFreeForAll { get; set; } = TimeSpan.FromSeconds(120);

    /// <summary>Delay between RouletteStarted and the server-random RouletteResult.</summary>
    public TimeSpan RouletteDelay { get; set; } = TimeSpan.FromSeconds(5);
}