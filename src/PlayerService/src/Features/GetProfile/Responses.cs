namespace PlayerService.Features.GetProfile;

/// <summary>
/// Contains player profile details and high-level gameplay statistics.
/// </summary>
/// <param name="Nickname">The display name used within the application.</param>
/// <param name="TotalGames">The total number of games participated in.</param>
/// <param name="Wins">The total number of games won (survived).</param>
/// <param name="Losses">The total number of games lost (eliminated).</param>
public record struct PlayerProfileResponse(
    string Nickname, 
    int TotalGames, 
    int Wins, 
    int Losses);
