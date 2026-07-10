namespace Bunker.GameService.Persistence.Values;

/// <summary>
/// A single vote cast during a Voting phase. <see cref="TargetId"/> is null for an abstain.
/// </summary>
public class VoteData
{
    public string VoterId { get; set; } = "";
    public string? TargetId { get; set; }
}