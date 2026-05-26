namespace Bunker.ContentService.Api.PersonalityPresets.Endpoints.Requests;

/// <summary>
/// Container for personality preset related HTTP request models.
/// </summary>
public abstract record PersonalityPresetRequest
{
    /// <summary>
    /// Request models for POST operations.
    /// </summary>
    public abstract record Post
    {
        /// <summary>
        /// Request to create a new personality preset.
        /// </summary>
        /// <param name="Title">The title of the preset. Minimum 4 characters.</param>
        /// <param name="Description">The description of the preset. Minimum 10 characters.</param>
        public readonly record struct Create(string Title, string Description);
    }

    /// <summary>
    /// Request models for PUT operations.
    /// </summary>
    public abstract record Put
    {
        /// <summary>
        /// Request to update an existing personality preset.
        /// </summary>
        /// <param name="Title">The new title of the preset. Minimum 4 characters.</param>
        /// <param name="Description">The new description of the preset. Minimum 10 characters.</param>
        public readonly record struct Update(string Title, string Description);
    }
}
