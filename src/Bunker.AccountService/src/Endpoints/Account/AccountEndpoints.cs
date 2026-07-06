using Microsoft.AspNetCore.Mvc;
using Wolverine;
using Microsoft.AspNetCore.Authorization;
using FluentValidation;
using Bunker.Api.Common.Identity;
using Bunker.AccountService.Features.GetProfile;
using Bunker.AccountService.Features.UpdateProfile;

namespace Bunker.AccountService.Endpoints;

internal static class AccountEndpoints
{
    public record UpdateProfileRequest(string Nickname);

    /// <summary>
    /// Retrieves the profile of the currently authenticated player.
    /// </summary>
    [Authorize]
    [ProducesResponseType<PlayerProfileResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    internal static async Task<IResult> GetMyProfile(
        [FromServices] IMessageBus bus,
        [FromServices] IUserIdentityContext identity,
        [FromServices] IValidator<GetProfile> validator)
    {
        if (identity.UserId is null)
            return TypedResults.Unauthorized();

        var query = new GetProfile(identity.UserId);
        var validationResult = await validator.ValidateAsync(query);
        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());

        var result = await bus.InvokeAsync<GetProfileResult>(query);

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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    internal static async Task<IResult> GetProfileById(
        [FromRoute] string id,
        [FromServices] IMessageBus bus,
        [FromServices] IValidator<GetProfile> validator)
    {
        var query = new GetProfile(id);
        var validationResult = await validator.ValidateAsync(query);
        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());

        var result = await bus.InvokeAsync<GetProfileResult>(query);

        return result switch
        {
            GetProfileSuccess success => TypedResults.Ok(success.Profile),
            GetProfileNotFound => TypedResults.NotFound(),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }

    /// <summary>
    /// Updates the nickname of the currently authenticated player.
    /// </summary>
    [Authorize]
    [ProducesResponseType<PlayerProfileResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    internal static async Task<IResult> UpdateMyProfile(
        [FromBody] UpdateProfileRequest body,
        [FromServices] IMessageBus bus,
        [FromServices] IUserIdentityContext identity,
        [FromServices] IValidator<UpdateProfile> validator)
    {
        if (identity.UserId is null)
            return TypedResults.Unauthorized();

        var command = new UpdateProfile(identity.UserId, body.Nickname);
        var validationResult = await validator.ValidateAsync(command);
        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());

        var result = await bus.InvokeAsync<UpdateProfileResult>(command);

        return result switch
        {
            UpdateProfileSuccess success => TypedResults.Ok(success.Profile),
            UpdateProfileNotFound => TypedResults.NotFound(),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }
}