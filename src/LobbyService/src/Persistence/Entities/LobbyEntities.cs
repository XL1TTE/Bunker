namespace LobbyService.Persistence.Entities;

public class LobbyEntity
{
    public required Guid Id { get; set; }
    public required string InviteCode { get; set; }
    public required int Capacity { get; set; }
    public bool IsPublic { get; set; }
    public List<LobbyParticipantEntity> Participants { get; set; } = [];
    public List<BotEntity> Bots { get; set; } = [];
    public List<LobbyCardPackEntity> Packs { get; set; } = [];
}

public class LobbyParticipantEntity
{
    public Guid Id { get; set; }
    public Guid LobbyId { get; set; }
    public Guid PlayerId { get; set; }
    public string Nickname { get; set; } = null!;
    public int Role { get; set; }
    public bool IsReady { get; set; }
}

public class BotEntity
{
    public string Name { get; set; } = null!;
    public string PersonalityPreset { get; set; } = null!;
}

public class LobbyCardPackEntity
{
    public Guid Id { get; set; }
    public Guid LobbyId { get; set; }
    public string PackId { get; set; } = null!;
}
