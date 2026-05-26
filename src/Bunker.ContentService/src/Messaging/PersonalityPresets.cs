namespace Bunker.ContentService.Messages;

/// <summary>
/// Event message sent when a personality preset is created or updated.
/// </summary>
/// <param name="Id">The unique identifier of the personality preset.</param>
/// <param name="Title">The title of the preset.</param>
/// <param name="Description">The description of the preset.</param>
public record PersonalityPresetUpdated(Guid Id, string Title, string Description);

/// <summary>
/// Event message sent when a personality preset is deleted.
/// </summary>
/// <param name="Id">The unique identifier of the deleted personality preset.</param>
public record PersonalityPresetDeleted(Guid Id);
