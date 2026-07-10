using Wolverine;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Bunker.ContentService.Domain;
using Bunker.ContentService.Features.Cards.CreateHealthCard;
using Bunker.ContentService.Features.Cards.GetHealthCards;
using Bunker.ContentService.Features.Cards.UpdateHealthCard;
using Bunker.ContentService.Api.Cards.Endpoints.Requests;
using Bunker.ContentService.Api.Cards.Endpoints.Responses;
using Bunker.ContentService.Transfers;

namespace Bunker.ContentService.Api.Cards.Endpoints;

internal static class HealthCardEndpoints
{
    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<CardResponse.HealthCard>(StatusCodes.Status200OK)]
    [ProducesResponseType<Microsoft.AspNetCore.Http.HttpResults.ValidationProblem>(StatusCodes.Status400BadRequest)]
    internal static async Task<IResult> CreateHealthCard(
        [FromBody] CardRequest.Post.HealthCard request,
        [FromServices] IMessageBus bus,
        [FromServices] IValidator<CardRequest.Post.HealthCard> validator)
    {
        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await bus.InvokeAsync<CreateHealthCard.Result>(new CreateHealthCard(request.Health));

        return result switch
        {
            CreateHealthCard.Result.Success success => TypedResults.Ok(new CardResponse.HealthCard(success.Card.ToTransferObject())),
            _ => throw new Exception("Unexpected error occurred during health card creation."),
        };
    }

    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<CardResponse.HealthCard>(StatusCodes.Status200OK)]
    [ProducesResponseType<Microsoft.AspNetCore.Http.HttpResults.ValidationProblem>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Microsoft.AspNetCore.Http.HttpResults.NotFound>(StatusCodes.Status404NotFound)]
    internal static async Task<IResult> UpdateHealthCard(
    [FromRoute] Guid id,
    [FromBody] CardRequest.Put.HealthCard request,
    [FromServices] IMessageBus bus,
    [FromServices] IValidator<CardRequest.Put.HealthCard> validator)
    {
        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await bus.InvokeAsync<UpdateHealthCard.Result>
            (new UpdateHealthCard(Id: Card.Id.Create(id), Health: request.Health));

        return result switch
        {
            UpdateHealthCard.Result.Success success => TypedResults.Ok(new CardResponse.HealthCard(success.Card.ToTransferObject())),
            Features.Cards.UpdateHealthCard.UpdateHealthCard.Result.NotFound => TypedResults.NotFound(),
            _ => throw new Exception("Unexpected error occurred during health card updating."),
        };
    }

    [ProducesResponseType<CardResponse.HealthCards>(StatusCodes.Status200OK)]
    internal static async Task<IResult> GetHealthCards(
        [FromServices] IMessageBus bus,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 10
    )
    {
        var result = await bus.InvokeAsync<GetHealthCards.Result>(new GetHealthCards(Skip: skip, Take: take));

        return result switch
        {
            GetHealthCards.Result.Success success
            => TypedResults.Ok(
                    new CardResponse.HealthCards(
                        Total: success.Total,
                        Cards: success.Cards.Select(x => x.ToTransferObject()))),

            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }
}