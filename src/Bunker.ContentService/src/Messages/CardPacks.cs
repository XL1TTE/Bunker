namespace Bunker.ContentService.Messages;

public record CardPackUpdated(Guid Id, string Title, string Description, IEnumerable<Guid> CardIds);
public record CardPackDeleted(Guid Id);
