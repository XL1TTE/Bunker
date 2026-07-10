namespace Bunker.GameService.Persistence.Values;

public class BunkerCardData
{
    public Guid Id { get; set; }
    public string Catastrophe { get; set; } = "";
    public string SurvivalDuration { get; set; } = "";
    public string BunkerEnvironment { get; set; } = "";
}