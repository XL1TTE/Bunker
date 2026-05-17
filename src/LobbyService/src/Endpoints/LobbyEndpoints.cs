using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LobbyService.Features.CreateLobby;
using LobbyService.Features.BrowseLobbies;
using LobbyService.Features.JoinLobby;
using LobbyService.Features.ToggleReady;
using LobbyService.Features.LeaveLobby;
using LobbyService.Features.UpdateSettings;
using LobbyService.Features.StartGame;
using Wolverine;
using FluentValidation;
using Bunker.Infrastructure.Identity;

namespace LobbyService.Endpoints;

internal static class LobbyEndpoints
{
    [Authorize]
    [ProducesResponseType<CreateLobbyResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    internal static async Task<IResult> CreateLobby(
        [FromBody] CreateLobbyRequest request,
        [FromServices] IValidator<CreateLobbyRequest> validator,
        [FromServices] IUserIdentityContext identity,
        [FromServices] IMessageBus bus)
    {
        if (identity.UserId is null || identity.Nickname is null)
            return TypedResults.Unauthorized();

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());

        var command = new CreateLobby(
            identity.UserId.Value,
            identity.Nickname,
            request.Capacity, 
            request.Bots, 
            request.Packs, 
            request.IsPublic);

        var result = await bus.InvokeAsync<CreateLobbyResult>(command);

        return result switch
        {
            CreateLobbySuccess success => TypedResults.Ok(success.Response),
            CreateLobbyUnauthorized => TypedResults.Unauthorized(),
            CreateLobbyFailure failure => TypedResults.BadRequest(failure.Reason),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }

    [Authorize]
    [ProducesResponseType<IEnumerable<LobbySummaryResponse>>(StatusCodes.Status200OK)]
    internal static async Task<IResult> BrowseLobbies(
        [FromServices] IMessageBus bus)
    {
        var result = await bus.InvokeAsync<BrowseLobbiesResult>(new BrowseLobbies());

        return result switch
        {
            BrowseLobbiesSuccess success => TypedResults.Ok(success.Lobbies),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }

    [Authorize]
    [ProducesResponseType<JoinLobbyResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    internal static async Task<IResult> JoinLobby(
        [FromBody] JoinLobbyRequest request,
        [FromServices] IValidator<JoinLobbyRequest> validator,
        [FromServices] IUserIdentityContext identity,
        [FromServices] IMessageBus bus)
    {
        if (identity.UserId is null || identity.Nickname is null)
            return TypedResults.Unauthorized();

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());

        var result = await bus.InvokeAsync<JoinLobbyResult>(new JoinLobby(
            identity.UserId.Value,
            identity.Nickname,
            request.InviteCode));

        return result switch
        {
            JoinLobbySuccess success => TypedResults.Ok(success.Response),
            JoinLobbyNotFound => TypedResults.NotFound(),
            JoinLobbyFull => TypedResults.Conflict("Lobby is full"),
            JoinLobbyAlreadyIn => TypedResults.Conflict("Player already in lobby"),
            JoinLobbyUnauthorized => TypedResults.Unauthorized(),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }

    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    internal static async Task<IResult> ToggleReady(
        [FromRoute] Guid id,
        [FromServices] IUserIdentityContext identity,
        [FromServices] IMessageBus bus)
    {
        if (identity.UserId is null)
            return TypedResults.Unauthorized();

        var result = await bus.InvokeAsync<ToggleReadyResult>(new ToggleReady(id, identity.UserId.Value));

        return result switch
        {
            ToggleReadySuccess => TypedResults.NoContent(),
            ToggleReadyNotFound => TypedResults.NotFound(),
            ToggleReadyPlayerNotInLobby => TypedResults.BadRequest("Player not in lobby"),
            ToggleReadyUnauthorized => TypedResults.Unauthorized(),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }

    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    internal static async Task<IResult> LeaveLobby(
        [FromRoute] Guid id,
        [FromServices] IUserIdentityContext identity,
        [FromServices] IMessageBus bus)
    {
        if (identity.UserId is null)
            return TypedResults.Unauthorized();

        var result = await bus.InvokeAsync<LeaveLobbyResult>(new LeaveLobby(id, identity.UserId.Value));

        return result switch
        {
            LeaveLobbySuccess => TypedResults.NoContent(),
            LeaveLobbyNotFound => TypedResults.NotFound(),
            LeaveLobbyUnauthorized => TypedResults.Unauthorized(),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }

    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    internal static async Task<IResult> UpdateSettings(
        [FromRoute] Guid id,
        [FromBody] UpdateSettingsRequest request,
        [FromServices] IValidator<UpdateSettingsRequest> validator,
        [FromServices] IUserIdentityContext identity,
        [FromServices] IMessageBus bus)
    {
        if (identity.UserId is null)
            return TypedResults.Unauthorized();

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());

        var command = new UpdateSettings(
            id,
            identity.UserId.Value,
            request.Capacity,
            request.Bots,
            request.Packs,
            request.IsPublic);

        var result = await bus.InvokeAsync<UpdateSettingsResult>(command);

        return result switch
        {
            UpdateSettingsSuccess => TypedResults.NoContent(),
            UpdateSettingsNotFound => TypedResults.NotFound(),
            UpdateSettingsForbidden => TypedResults.Forbid(),
            UpdateSettingsFailure failure => TypedResults.BadRequest(failure.Reason),
            UpdateSettingsUnauthorized => TypedResults.Unauthorized(),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }

    [Authorize]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    internal static async Task<IResult> StartGame(
        [FromRoute] Guid id,
        [FromServices] IUserIdentityContext identity,
        [FromServices] IMessageBus bus)
    {
        if (identity.UserId is null)
            return TypedResults.Unauthorized();

        var result = await bus.InvokeAsync<StartGameResult>(new StartGame(id, identity.UserId.Value));

        return result switch
        {
            StartGameSuccess => TypedResults.Accepted(""),
            StartGameNotFound => TypedResults.NotFound(),
            StartGameForbidden => TypedResults.Forbid(),
            StartGameNotReady => TypedResults.BadRequest("Not all players are ready"),
            StartGameUnauthorized => TypedResults.Unauthorized(),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }
}

