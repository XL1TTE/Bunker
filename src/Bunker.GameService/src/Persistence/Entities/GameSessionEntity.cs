namespace Bunker.GameService.Persistence.Entities;

public class GameSessionEntity
{
    public Guid GameId { get; set; }
    public Guid LobbyId { get; set; }
    public string HostId { get; set; } = "";
    public string Status { get; set; } = "Started";
    public DateTime CreatedAt { get; set; }
}