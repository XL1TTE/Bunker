namespace Bunker.AccountService.Features.GetProfile;

/// <summary>
/// Contains player profile details and high-level gameplay statistics.
/// </summary>
/// <param name="Id">The unique identifier of the player.</param>
/// <param name="Nickname">The display name used within the application.</param>
/// <param name="TotalGames">The total number of games participated in.</param>
/// <param name="Wins">The total number of games won (survived).</param>
/// <param name="Losses">The total number of games lost (eliminated).</param>
public record struct PlayerProfileResponse(
    string Id,
    string Nickname,
    int TotalGames,
    int Wins,
    int Losses);