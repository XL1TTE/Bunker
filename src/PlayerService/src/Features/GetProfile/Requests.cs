namespace PlayerService.Features.GetProfile;

/// <summary>
/// Represents a request to retrieve a player's profile information.
/// </summary>
/// <param name="Id">The unique identifier of the player. Must be a valid GUID string.</param>
public record GetProfile(string Id);
