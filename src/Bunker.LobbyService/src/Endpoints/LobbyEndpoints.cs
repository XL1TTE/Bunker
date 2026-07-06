using Bunker.Api.Common.Identity;
using Bunker.LobbyService.Domain;
using Bunker.LobbyService.Features.AddLobbyBot;
using Bunker.LobbyService.Features.CreateLobby;
using Bunker.LobbyService.Features.JoinLobby;
using Bunker.LobbyService.Features.KickLobbyParticipant;
using Bunker.LobbyService.Features.LeaveLobby;
using Bunker.LobbyService.Features.PlayLobby;
using Bunker.LobbyService.Features.RemoveLobbyBot;
using Bunker.LobbyService.Features.SendLobbyMessage;
using Bunker.LobbyService.Features.ToggleLobbyReadiness;
using Bunker.LobbyService.Features.UpdateLobbySettings;
using Bunker.LobbyService.Persistence.Queries;
using Bunker.LobbyService.Transfers;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Bunker.LobbyService.Api.Endpoints;

internal static class LobbyEndpoints
{
    [Authorize]
    [ProducesResponseType<Transfer.LobbySnapshot>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblem>(StatusCodes.Status400BadRequest)]
    public static async Task<IResult> Create(
        [FromBody] Request.CreateLobby request,
        [FromServices] IMessageBus bus,
        [FromServices] IUserIdentityContext identity,
        [FromServices] IValidator<Request.CreateLobby> validator)
    {
        var validation = validator.Validate(request);
        if (validation.IsValid is false)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        if (identity.UserId is null)
            return TypedResults.Unauthorized();

        var result = await bus.InvokeAsync<CreateLobby.Result>(new CreateLobby(
            HostId: identity.UserId,
            Nickname: identity.Nickname ?? "Anonymous",
            Capacity: request.Capacity,
            IsPublic: request.IsPublic,
            Password: request.Password,
            SelectedPackIds: request.SelectedPackIds
        ));

        return result switch
        {
            CreateLobby.Result.Success success => TypedResults.Created(
                $"/lobbies/{success.Lobby.PublicId.Value}",
                success.Lobby.ToTransfer()),
            CreateLobby.Result.Failure failure => TypedResults.BadRequest(failure.Error),
            _ => TypedResults.Problem("An unexpected error occurred.")
        };
    }

    [Authorize]
    [ProducesResponseType<Transfer.LobbySnapshot>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    public static async Task<IResult> JoinByInviteCode(
        [FromRoute] string inviteCode,
        [FromServices] IMessageBus bus,
        [FromServices] IUserIdentityContext identity)
    {
        if (identity.UserId is null)
            return TypedResults.Unauthorized();

        var result = await bus.InvokeAsync<JoinLobby.Result>(new JoinByInviteCode(
            InviteCode: inviteCode,
            CallerId: identity.UserId,
            Nickname: identity.Nickname ?? "Anonymous"));

        return result switch
        {
            JoinLobby.Result.Success success => TypedResults.Ok(success.Lobby.ToTransfer()),
            JoinLobby.Result.Failure failure => TypedResults.BadRequest(failure.Error),
            _ => TypedResults.Problem("An unexpected error occurred.")
        };
    }

    [Authorize]
    [ProducesResponseType<Transfer.LobbySnapshot>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblem>(StatusCodes.Status400BadRequest)]
    public static async Task<IResult> JoinByPassword(
        [FromRoute] Guid lobbyId,
        [FromBody] Request.JoinByPasswordRequest request,
        [FromServices] IMessageBus bus,
        [FromServices] IUserIdentityContext identity)
    {
        if (identity.UserId is null)
            return TypedResults.Unauthorized();

        var result = await bus.InvokeAsync<JoinLobby.Result>(new JoinByPassword(
            LobbyId: lobbyId,
            Password: request.Password,
            CallerId: identity.UserId,
            Nickname: identity.Nickname ?? "Anonymous"));

        return result switch
        {
            JoinLobby.Result.Success success => TypedResults.Ok(success.Lobby.ToTransfer()),
            JoinLobby.Result.Failure failure => TypedResults.BadRequest(failure.Error),
            _ => TypedResults.Problem("An unexpected error occurred.")
        };
    }

    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    public static async Task<IResult> Leave(
        [FromRoute] Guid lobbyId,
        [FromServices] IMessageBus bus,
        [FromServices] IUserIdentityContext identity)
    {
        if (identity.UserId is null)
            return TypedResults.Unauthorized();

        var result = await bus.InvokeAsync<LeaveLobby.Result>(
            new LeaveLobby(LobbyId: lobbyId.ToString(), CallerId: identity.UserId));

        return result switch
        {
            LeaveLobby.Result.Success => TypedResults.NoContent(),
            LeaveLobby.Result.Failure failure => TypedResults.BadRequest(failure.Error),
            _ => TypedResults.Problem("An unexpected error occurred.")
        };
    }

    [Authorize]
    [ProducesResponseType<Transfer.LobbySnapshot>(StatusCodes.Status200OK)]
    public static async Task<IResult> GetOne(
        [FromRoute] Guid lobbyId,
        [FromServices] ILobbyQueries queries)
    {
        var lobby = await queries.GetByIdAsync(Lobby.Id.Restore(lobbyId));
        return lobby is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(lobby.ToTransfer());
    }

    [Authorize]
    [ProducesResponseType<Transfer.LobbyListResponse>(StatusCodes.Status200OK)]
    public static async Task<IResult> List(
        [FromQuery] int limit,
        [FromQuery] int offset,
        [FromServices] ILobbyQueries queries)
    {
        var (items, total) = await queries.ListPublicAsync(limit, offset);
        return TypedResults.Ok(new Transfer.LobbyListResponse(
            Items: items.Select(l => l.ToSummary()).ToArray(),
            Total: total
        ));
    }

    [Authorize]
    [ProducesResponseType<Transfer.LobbySnapshot>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblem>(StatusCodes.Status400BadRequest)]
    public static async Task<IResult> UpdateSettings(
        [FromRoute] Guid lobbyId,
        [FromBody] Request.UpdateSettingsRequest request,
        [FromServices] IMessageBus bus,
        [FromServices] IUserIdentityContext identity,
        [FromServices] IValidator<Request.UpdateSettingsRequest> validator)
    {
        var validation = validator.Validate(request);
        if (validation.IsValid is false)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        if (identity.UserId is null)
            return TypedResults.Unauthorized();

        var result = await bus.InvokeAsync<UpdateLobbySettings.Result>(new UpdateLobbySettings(
            LobbyId: lobbyId.ToString(),
            CallerId: identity.UserId,
            Capacity: request.Capacity,
            IsPublic: request.IsPublic,
            Password: request.Password,
            SelectedPackIds: request.SelectedPackIds
        ));

        return result switch
        {
            UpdateLobbySettings.Result.Success success => TypedResults.Ok(success.Lobby.ToTransfer()),
            UpdateLobbySettings.Result.Failure failure => TypedResults.BadRequest(failure.Error),
            _ => TypedResults.Problem("An unexpected error occurred.")
        };
    }

    [Authorize]
    [ProducesResponseType<Transfer.LobbySnapshot>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblem>(StatusCodes.Status400BadRequest)]
    public static async Task<IResult> AddBot(
        [FromRoute] Guid lobbyId,
        [FromBody] Request.AddBotRequest request,
        [FromServices] IMessageBus bus,
        [FromServices] IUserIdentityContext identity,
        [FromServices] IValidator<Request.AddBotRequest> validator)
    {
        var validation = validator.Validate(request);
        if (validation.IsValid is false)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        if (identity.UserId is null)
            return TypedResults.Unauthorized();

        var result = await bus.InvokeAsync<AddLobbyBot.Result>(new AddLobbyBot(
            LobbyId: lobbyId.ToString(),
            CallerId: identity.UserId,
            PersonalityPresetId: request.PersonalityPresetId,
            Nickname: request.Nickname
        ));

        return result switch
        {
            AddLobbyBot.Result.Success success => TypedResults.Ok(success.Lobby.ToTransfer()),
            AddLobbyBot.Result.Failure failure => TypedResults.BadRequest(failure.Error),
            _ => TypedResults.Problem("An unexpected error occurred.")
        };
    }

    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    public static async Task<IResult> RemoveBot(
        [FromRoute] Guid lobbyId,
        [FromRoute] Guid participantId,
        [FromServices] IMessageBus bus,
        [FromServices] IUserIdentityContext identity)
    {
        if (identity.UserId is null)
            return TypedResults.Unauthorized();

        var result = await bus.InvokeAsync<RemoveLobbyBot.Result>(new RemoveLobbyBot(
            LobbyId: lobbyId.ToString(),
            CallerId: identity.UserId,
            ParticipantId: participantId
        ));

        return result switch
        {
            RemoveLobbyBot.Result.Success => TypedResults.NoContent(),
            RemoveLobbyBot.Result.Failure failure => TypedResults.BadRequest(failure.Error),
            _ => TypedResults.Problem("An unexpected error occurred.")
        };
    }

    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    public static async Task<IResult> KickParticipant(
        [FromRoute] Guid lobbyId,
        [FromRoute] Guid participantId,
        [FromServices] IMessageBus bus,
        [FromServices] IUserIdentityContext identity)
    {
        if (identity.UserId is null)
            return TypedResults.Unauthorized();

        var result = await bus.InvokeAsync<KickLobbyParticipant.Result>(new KickLobbyParticipant(
            LobbyId: lobbyId.ToString(),
            CallerId: identity.UserId,
            ParticipantId: participantId
        ));

        return result switch
        {
            KickLobbyParticipant.Result.Success => TypedResults.NoContent(),
            KickLobbyParticipant.Result.Failure failure => TypedResults.BadRequest(failure.Error),
            _ => TypedResults.Problem("An unexpected error occurred.")
        };
    }

    [Authorize]
    [ProducesResponseType<Transfer.LobbySnapshot>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    public static async Task<IResult> ToggleReadiness(
        [FromRoute] Guid lobbyId,
        [FromServices] IMessageBus bus,
        [FromServices] IUserIdentityContext identity)
    {
        if (identity.UserId is null)
            return TypedResults.Unauthorized();

        var result = await bus.InvokeAsync<ToggleLobbyReadiness.Result>(new ToggleLobbyReadiness(
            LobbyId: lobbyId.ToString(),
            CallerId: identity.UserId
        ));

        return result switch
        {
            ToggleLobbyReadiness.Result.Success success => TypedResults.Ok(success.Lobby.ToTransfer()),
            ToggleLobbyReadiness.Result.Failure failure => TypedResults.BadRequest(failure.Error),
            _ => TypedResults.Problem("An unexpected error occurred.")
        };
    }

    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    public static async Task<IResult> Start(
        [FromRoute] Guid lobbyId,
        [FromServices] IMessageBus bus,
        [FromServices] IUserIdentityContext identity)
    {
        if (identity.UserId is null)
            return TypedResults.Unauthorized();

        var result = await bus.InvokeAsync<PlayLobby.Result>(
            new PlayLobby(LobbyId: lobbyId.ToString(), HostId: identity.UserId));

        return result switch
        {
            PlayLobby.Result.Success => TypedResults.NoContent(),
            PlayLobby.Result.Failure failure => TypedResults.BadRequest(failure.Error),
            _ => TypedResults.Problem("An unexpected error occurred.")
        };
    }

    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblem>(StatusCodes.Status400BadRequest)]
    public static async Task<IResult> SendMessage(
        [FromRoute] Guid lobbyId,
        [FromBody] Request.SendMessageRequest request,
        [FromServices] IMessageBus bus,
        [FromServices] IUserIdentityContext identity,
        [FromServices] IValidator<Request.SendMessageRequest> validator)
    {
        var validation = validator.Validate(request);
        if (validation.IsValid is false)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        if (identity.UserId is null)
            return TypedResults.Unauthorized();

        var result = await bus.InvokeAsync<SendLobbyMessage.Result>(new SendLobbyMessage(
            LobbyId: lobbyId.ToString(),
            CallerId: identity.UserId,
            Text: request.Text
        ));

        return result switch
        {
            SendLobbyMessage.Result.Success => TypedResults.NoContent(),
            SendLobbyMessage.Result.Failure failure => TypedResults.BadRequest(failure.Error),
            _ => TypedResults.Problem("An unexpected error occurred.")
        };
    }
}
