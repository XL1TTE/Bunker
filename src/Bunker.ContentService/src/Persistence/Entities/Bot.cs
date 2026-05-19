namespace Bunker.ContentService.Persistence.Entities;

public class PersonalityPreset
{
    public required Guid PublicId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
}
