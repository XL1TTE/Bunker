namespace Bunker.ContentService.Messages;

/// <summary>
/// Event message sent when a canned bunker card is created or updated.
/// </summary>
/// <param name="Id">The unique identifier of the bunker card.</param>
/// <param name="Catastrophe">What happened to the outside world.</param>
/// <param name="SurvivalDuration">The time horizon players must justify surviving.</param>
/// <param name="BunkerEnvironment">Free-prose description of what the bunker has and lacks.</param>
public record BunkerCardUpdated(Guid Id, string Catastrophe, string SurvivalDuration, string BunkerEnvironment);

/// <summary>
/// Event message sent when a canned bunker card is deleted.
/// </summary>
/// <param name="Id">The unique identifier of the deleted bunker card.</param>
public record BunkerCardDeleted(Guid Id);