namespace Bunker.GameService.Persistence.Values;

public class ParticipantData
{
    public string Id { get; set; } = "";
    public string Nickname { get; set; } = "";
    public string Type { get; set; } = "";
    public Guid? PersonalityPresetId { get; set; }
    public string? AccountId { get; set; }
    public bool Eliminated { get; set; }
    public List<AttributeSlotData> Attributes { get; set; } = [];
}