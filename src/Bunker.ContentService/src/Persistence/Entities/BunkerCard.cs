namespace Bunker.ContentService.Persistence.Entities;

public class BunkerCard
{
    public required Guid PublicId { get; set; }
    public required string Catastrophe { get; set; }
    public required string SurvivalDuration { get; set; }
    public required string BunkerEnvironment { get; set; }
}