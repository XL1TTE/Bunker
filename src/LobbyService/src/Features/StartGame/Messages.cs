namespace LobbyService.Features.StartGame;

public record PlayerConfiguration(Guid PlayerId, string Nickname);
public record BotConfiguration(string Name, string PersonalityPreset);

public record PrepareGameRequested(
    Guid LobbyId,
    IEnumerable<PlayerConfiguration> Players,
    IEnumerable<BotConfiguration> Bots,
    IEnumerable<string> Packs);
