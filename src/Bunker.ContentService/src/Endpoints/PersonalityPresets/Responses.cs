using Bunker.ContentService.Transfers;

namespace Bunker.ContentService.Api.PersonalityPresets.Endpoints.Responses;

/// <summary>
/// Container for personality preset related HTTP response models.
/// </summary>
public abstract record PersonalityPresetResponse
{
    /// <summary>
    /// Response indicating that a personality preset was successfully created.
    /// </summary>
    /// <param name="Preset">The created personality preset data.</param>
    public readonly record struct Created(Transfer.PersonalityPreset Preset);

    /// <summary>
    /// Response indicating that a personality preset was successfully updated.
    /// </summary>
    /// <param name="Preset">The updated personality preset data.</param>
    public readonly record struct Updated(Transfer.PersonalityPreset Preset);

    /// <summary>
    /// Response containing a single personality preset.
    /// </summary>
    /// <param name="Preset">The personality preset data.</param>
    public readonly record struct Single(Transfer.PersonalityPreset Preset);

    /// <summary>
    /// Response containing a collection of all personality presets.
    /// </summary>
    /// <param name="Presets">The collection of personality presets.</param>
    public readonly record struct All(IEnumerable<Transfer.PersonalityPreset> Presets);
}
