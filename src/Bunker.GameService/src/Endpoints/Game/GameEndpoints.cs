using Bunker.Api.Common.Identity;
using Bunker.GameService.Features.CastVote;
using Bunker.GameService.Features.LeaveGame;
using Bunker.GameService.Features.RevealAttribute;
using Bunker.GameService.Features.SendGameMessage;
using Bunker.GameService.Persistence.Contracts.Queries;
using Bunker.GameService.Transfers;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Bunker.GameService.Api.Endpoints;

internal static class GameEndpoints
{
    [Authorize]
    [ProducesResponseType<GameSnapshot>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    internal static async Task<IResult> GetOne(
        [FromRoute] Guid gameId,
        [FromServices] IGameQueries queries,
        [FromServices] IUserIdentityContext identity)
    {
        if (identity.UserId is null)
            return TypedResults.Unauthorized();

        var game = await queries.GetGameAsync(gameId);
        if (game is null)
            return TypedResults.NotFound();

        var viewerInGame = game.Participants.Any(p => p.AccountId == identity.UserId);
        if (!viewerInGame)
            return TypedResults.Forbid();

        return TypedResults.Ok(GameSnapshotMapper.ToSnapshot(game, identity.UserId));
    }

    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblem>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    internal static async Task<IResult> Reveal(
        [FromRoute] Guid gameId,
        [FromBody] Request.RevealAttribute request,
        [FromServices] IMessageBus bus,
        [FromServices] IUserIdentityContext identity,
        [FromServices] IValidator<Request.RevealAttribute> validator)
    {
        var validation = validator.Validate(request);
        if (validation.IsValid is false)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        if (identity.UserId is not { } userId)
            return TypedResults.Unauthorized();

        var result = await bus.InvokeAsync<RevealAttribute.Result>(
            new RevealAttribute(GameId: gameId, AccountId: userId, AttributeKind: request.AttributeKind));

        return result switch
        {
            RevealAttribute.Result.Success => TypedResults.NoContent(),
            RevealAttribute.Result.Failure failure => TypedResults.BadRequest(failure.Error),
            _ => TypedResults.Problem("An unexpected error occurred.")
        };
    }

    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblem>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    internal static async Task<IResult> SendMessage(
        [FromRoute] Guid gameId,
        [FromBody] Request.SendMessageRequest request,
        [FromServices] IMessageBus bus,
        [FromServices] IUserIdentityContext identity,
        [FromServices] IValidator<Request.SendMessageRequest> validator)
    {
        var validation = validator.Validate(request);
        if (validation.IsValid is false)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        if (identity.UserId is not { } userId)
            return TypedResults.Unauthorized();

        var result = await bus.InvokeAsync<SendGameMessage.Result>(
            new SendGameMessage(GameId: gameId, AccountId: userId, Text: request.Text));

        return result switch
        {
            SendGameMessage.Result.Success => TypedResults.NoContent(),
            SendGameMessage.Result.Failure failure => TypedResults.BadRequest(failure.Error),
            _ => TypedResults.Problem("An unexpected error occurred.")
        };
    }

    [Authorize]
    [ProducesResponseType<IReadOnlyList<ChatMessageDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    internal static async Task<IResult> GetMessages(
        [FromRoute] Guid gameId,
        [FromServices] IGameQueries queries,
        [FromServices] IUserIdentityContext identity)
    {
        if (identity.UserId is null)
            return TypedResults.Unauthorized();

        var game = await queries.GetGameAsync(gameId);
        if (game is null)
            return TypedResults.NotFound();

        var viewerInGame = game.Participants.Any(p => p.AccountId == identity.UserId);
        if (!viewerInGame)
            return TypedResults.Forbid();

        return TypedResults.Ok(await queries.GetChatMessagesAsync(gameId));
    }

    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblem>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    internal static async Task<IResult> Vote(
        [FromRoute] Guid gameId,
        [FromBody] Request.VoteRequest request,
        [FromServices] IMessageBus bus,
        [FromServices] IUserIdentityContext identity,
        [FromServices] IValidator<Request.VoteRequest> validator)
    {
        var validation = validator.Validate(request);
        if (validation.IsValid is false)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        if (identity.UserId is not { } userId)
            return TypedResults.Unauthorized();

        var result = await bus.InvokeAsync<CastVote.Result>(
            new CastVote(GameId: gameId, AccountId: userId, TargetParticipantId: request.TargetParticipantId));

        return result switch
        {
            CastVote.Result.Success => TypedResults.NoContent(),
            CastVote.Result.Failure failure => TypedResults.BadRequest(failure.Error),
            _ => TypedResults.Problem("An unexpected error occurred.")
        };
    }

    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    internal static async Task<IResult> Leave(
        [FromRoute] Guid gameId,
        [FromServices] IMessageBus bus,
        [FromServices] IUserIdentityContext identity)
    {
        if (identity.UserId is not { } userId)
            return TypedResults.Unauthorized();

        var result = await bus.InvokeAsync<LeaveGame.Result>(
            new LeaveGame(GameId: gameId, AccountId: userId));

        return result switch
        {
            LeaveGame.Result.Success => TypedResults.NoContent(),
            LeaveGame.Result.Failure failure => TypedResults.BadRequest(failure.Error),
            _ => TypedResults.Problem("An unexpected error occurred.")
        };
    }
}