namespace Bunker.AccountService.Features.UpdateProfile;

/// <param name="Id">The unique identifier of the player whose nickname is being updated.</param>
/// <param name="Nickname">The new display name for the player.</param>
public record UpdateProfile(string Id, string Nickname);