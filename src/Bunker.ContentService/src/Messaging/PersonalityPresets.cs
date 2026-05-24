namespace Bunker.ContentService.Messages;

public record PersonalityPresetUpdated(Guid Id, string Title, string Description);
public record PersonalityPresetDeleted(Guid Id);
