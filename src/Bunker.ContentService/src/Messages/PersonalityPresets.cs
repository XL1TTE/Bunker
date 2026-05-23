namespace Bunker.ContentService.Messages;

public record PersonalityPresetUpdated(Guid PublicId, string Title, string Description);
public record PersonalityPresetDeleted(Guid PublicId);
