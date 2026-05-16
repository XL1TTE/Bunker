using Microsoft.AspNetCore.Mvc;
using Wolverine;
using PlayerService.Features.GetProfile;
using Bunker.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;

namespace PlayerService.Endpoints;

internal static class PlayerEndpoints
{
    /// <summary>
    /// Retrieves the profile of the currently authenticated player.
    /// </summary>
    [Authorize]
    [ProducesResponseType<PlayerProfileResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    internal static async Task<IResult> GetMyProfile(
        [FromServices] IMessageBus bus,
        [FromServices] IUserIdentityContext identity)
    {
        var result = await bus.InvokeAsync<GetProfileResult>(new GetProfile(identity.UserId!));

        return result switch
        {
            GetProfileSuccess success => TypedResults.Ok(success.Profile),
            GetProfileNotFound => TypedResults.NotFound(),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }

    /// <summary>
    /// Retrieves the profile of a player by their ID.
    /// </summary>
    [Authorize]
    [ProducesResponseType<PlayerProfileResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    internal static async Task<IResult> GetProfileById(
        [FromRoute] string id,
        [FromServices] IMessageBus bus)
    {
        var result = await bus.InvokeAsync<GetProfileResult>(new GetProfile(id));

        return result switch
        {
            GetProfileSuccess success => TypedResults.Ok(success.Profile),
            GetProfileNotFound => TypedResults.NotFound(),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }
}
