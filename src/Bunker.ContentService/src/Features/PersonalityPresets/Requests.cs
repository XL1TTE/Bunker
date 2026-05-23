namespace Bunker.ContentService.Features.PersonalityPresets;

/// <summary>Request to create a new Personality Preset for bots.</summary>
public record CreatePersonalityPreset(string Title, string Description);
/// <summary>Request to update an existing Personality Preset.</summary>
public record UpdatePersonalityPreset(Guid Id, string Title, string Description);
/// <summary>Request to delete a Personality Preset.</summary>
public record DeletePersonalityPreset(Guid Id);
/// <summary>Request to retrieve a specific Personality Preset by its ID.</summary>
public record GetPersonalityPreset(Guid Id);
/// <summary>Request to retrieve all Personality Presets.</summary>
public record GetAllPersonalityPresets();
