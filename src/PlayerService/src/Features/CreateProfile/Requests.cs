namespace PlayerService.Features.CreateProfile;

/// <summary>
/// Represents a request to initialize a new persistent player profile.
/// </summary>
/// <param name="Id">The unique identifier to be assigned to the profile.</param>
/// <param name="Nickname">The initial display name for the player.</param>
public record CreateProfile(Guid Id, string Nickname);
