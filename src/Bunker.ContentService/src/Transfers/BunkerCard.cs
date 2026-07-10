namespace Bunker.ContentService.Transfers;

public abstract partial class Transfer
{
    /// <summary>
    /// Transfer object of a canned Bunker Card.
    /// </summary>
    /// <param name="Id">Id of the bunker card.</param>
    /// <param name="Catastrophe">What happened to the outside world.</param>
    /// <param name="SurvivalDuration">The time horizon players must justify surviving.</param>
    /// <param name="BunkerEnvironment">Free-prose description of what the bunker has and lacks.</param>
    public readonly record struct BunkerCard(Guid Id, string Catastrophe, string SurvivalDuration, string BunkerEnvironment);
}