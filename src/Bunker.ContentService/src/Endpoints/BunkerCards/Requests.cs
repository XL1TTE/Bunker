namespace Bunker.ContentService.Api.BunkerCards.Endpoints.Requests;

/// <summary>
/// Container for bunker card related HTTP request models.
/// </summary>
public abstract record BunkerCardRequest
{
    /// <summary>
    /// Request models for POST operations.
    /// </summary>
    public abstract record Post
    {
        /// <summary>
        /// Request to create a new canned bunker card.
        /// </summary>
        /// <param name="Catastrophe">What happened to the outside world. Minimum 8 characters.</param>
        /// <param name="SurvivalDuration">The time horizon players must justify surviving. Minimum 3 characters.</param>
        /// <param name="BunkerEnvironment">Free-prose description of what the bunker has and lacks. Minimum 10 characters.</param>
        public readonly record struct Create(string Catastrophe, string SurvivalDuration, string BunkerEnvironment);
    }

    /// <summary>
    /// Request models for PUT operations.
    /// </summary>
    public abstract record Put
    {
        /// <summary>
        /// Request to update an existing canned bunker card.
        /// </summary>
        /// <param name="Catastrophe">The new catastrophe. Minimum 8 characters.</param>
        /// <param name="SurvivalDuration">The new survival duration. Minimum 3 characters.</param>
        /// <param name="BunkerEnvironment">The new bunker environment. Minimum 10 characters.</param>
        public readonly record struct Update(string Catastrophe, string SurvivalDuration, string BunkerEnvironment);
    }
}