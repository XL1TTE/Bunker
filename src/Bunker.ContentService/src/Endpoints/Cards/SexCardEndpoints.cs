using Wolverine;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Bunker.ContentService.Domain;
using Bunker.ContentService.Features.Cards.CreateSexCard;
using Bunker.ContentService.Features.Cards.GetSexCards;
using Bunker.ContentService.Features.Cards.UpdateSexCard;
using Bunker.ContentService.Api.Cards.Endpoints.Requests;
using Bunker.ContentService.Api.Cards.Endpoints.Responses;
using Bunker.ContentService.Transfers;
using System.Text.Json;

namespace Bunker.ContentService.Api.Cards.Endpoints;

internal static class SexCardEndpoints
{
    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<CardResponse.SexCard>(StatusCodes.Status200OK)]
    [ProducesResponseType<Microsoft.AspNetCore.Http.HttpResults.ValidationProblem>(StatusCodes.Status400BadRequest)]
    internal static async Task<IResult> CreateSexCard(
        [FromBody] CardRequest.Post.SexCard request,
        [FromServices] IMessageBus bus,
        [FromServices] ILogger<CardRequest.Post.SexCard> logger,
        [FromServices] IValidator<CardRequest.Post.SexCard> validator)
    {
        var validation = await validator.ValidateAsync(request);

        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await bus.InvokeAsync<CreateSexCard.Result>(new CreateSexCard(request.Sex));

        return result switch
        {
            CreateSexCard.Result.Success success => TypedResults.Ok(new CardResponse.SexCard(success.Card.ToTransferObject())),
            _ => throw new Exception("Unexpected error occurred during sex card creation."),
        };
    }

    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<CardResponse.SexCard>(StatusCodes.Status200OK)]
    [ProducesResponseType<Microsoft.AspNetCore.Http.HttpResults.ValidationProblem>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Microsoft.AspNetCore.Http.HttpResults.NotFound>(StatusCodes.Status404NotFound)]
    internal static async Task<IResult> UpdateSexCard(
    [FromRoute] Guid id,
    [FromBody] CardRequest.Put.SexCard request,
    [FromServices] IMessageBus bus,
    [FromServices] IValidator<CardRequest.Put.SexCard> validator)
    {
        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await bus.InvokeAsync<UpdateSexCard.Result>
            (new UpdateSexCard(Id: Card.Id.Create(id), Sex: request.Sex));

        return result switch
        {
            UpdateSexCard.Result.Success success => TypedResults.Ok(new CardResponse.SexCard(success.Card.ToTransferObject())),
            Features.Cards.UpdateSexCard.UpdateSexCard.Result.NotFound => TypedResults.NotFound(),
            _ => throw new Exception("Unexpected error occurred during sex card updating."),
        };
    }

    [ProducesResponseType<CardResponse.SexCards>(StatusCodes.Status200OK)]
    internal static async Task<IResult> GetSexCards([FromServices] IMessageBus bus)
    {
        var result = await bus.InvokeAsync<GetSexCards.Result>(new GetSexCards());

        return result switch
        {
            GetSexCards.Result.Success success
            => TypedResults.Ok(
                    new CardResponse.SexCards(
                        Total: success.Total,
                        Cards: success.Cards.Select(x => x.ToTransferObject()))),

            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }
}
