using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Wolverine;
using Microsoft.AspNetCore.Http.HttpResults;
using Bunker.LobbyService.Features.CreateLobby;
using Bunker.Api.Common.Identity;
using Microsoft.AspNetCore.Authorization;
using Bunker.LobbyService.Transfers;

namespace Bunker.LobbyService.Api.Endpoints;

internal static class LobbyEndpoints
{
    [Authorize]
    [ProducesResponseType<Responses.CreatedLobby>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblem>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<UnauthorizedHttpResult>(StatusCodes.Status401Unauthorized)]
    public static async Task<IResult> Create(
        [FromBody] Request.CreateLobby request,
        [FromServices] IMessageBus bus,
        [FromServices] IUserIdentityContext identity,
        [FromServices] IValidator<Request.CreateLobby> validator
    )
    {
        var validation = validator.Validate(request);
        if (validation.IsValid is false)
        {
            return TypedResults.ValidationProblem(validation.ToDictionary());
        }

        if (identity.IsAuthenticated is false)
        {
            return TypedResults.Unauthorized();
        }

        var result = await bus.InvokeAsync<CreateLobby.Result>(new CreateLobby(
            Capacity: request.Capacity,
            Visible: request.Visible,
            LobbyPassword: request.LobbyPassword,
            HostId: identity.UserId!,
            Nickname: identity.Nickname ?? "Anonymous",
            CardPackIds: request.CardPackIds
        ));

        return result switch
        {
            CreateLobby.Result.Success success => TypedResults.Created(
                $"/lobbies/{success.Lobby.PublicId.Value}",
                new Responses.CreatedLobby(LobbySnapshot: success.Lobby.ToTransfer())),
            CreateLobby.Result.Failure failure => TypedResults.BadRequest(failure.Error),
            _ => TypedResults.Problem("An unexpected error occurred.")
        };
    }
}
