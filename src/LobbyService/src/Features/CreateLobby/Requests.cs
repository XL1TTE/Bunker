namespace LobbyService.Features.CreateLobby;

/// <summary>
/// Represents a configuration for an AI bot to be added to the lobby.
/// </summary>
/// <param name="Name">The display name of the bot.</param>
/// <param name="PersonalityPresetId">The unique identifier of the personality profile that governs the bot's social behavior and decision-making.</param>
public record BotRequest(string Name, Guid PersonalityPresetId);

/// <summary>
/// Data required to initialize a new game lobby.
/// </summary>
/// <param name="Capacity">The maximum total number of participants (players and bots) allowed in the lobby.</param>
/// <param name="Bots">A collection of AI bots to be included in the game.</param>
/// <param name="Packs">A list of identifiers for card packs to be used in the game session.</param>
/// <param name="IsPublic">Determines if the lobby is visible in the public lobby browser.</param>
public record CreateLobbyRequest(
    int Capacity, 
    IEnumerable<BotRequest> Bots, 
    IEnumerable<string> Packs, 
    bool IsPublic);

/// <summary>
/// Information about the successfully created lobby.
/// </summary>
/// <param name="LobbyId">The unique identifier of the created lobby.</param>
/// <param name="InviteCode">The unique 6-character code used by other players to join the lobby.</param>
public record CreateLobbyResponse(Guid LobbyId, string InviteCode);
