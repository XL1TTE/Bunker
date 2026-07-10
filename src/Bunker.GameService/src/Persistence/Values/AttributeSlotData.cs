namespace Bunker.GameService.Persistence.Values;

public class AttributeSlotData
{
    public string Kind { get; set; } = "";
    public Guid CardId { get; set; }
    public string Value { get; set; } = "";
    public bool Revealed { get; set; }
}