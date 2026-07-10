namespace Bunker.GameService.Persistence.Entities;

/// <summary>
/// A persisted discussion-chat message for a game (ADR 0004: chat is not saga state).
/// Kept so a page reload can recover the conversation history.
/// </summary>
public class GameChatMessage
{
    public Guid Id { get; set; }
    public Guid GameId { get; set; }
    public string ParticipantId { get; set; } = "";
    public string Nickname { get; set; } = "";
    public string Text { get; set; } = "";
    public DateTime SentAt { get; set; }
}