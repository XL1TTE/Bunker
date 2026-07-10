using Wolverine;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Bunker.ContentService.Domain;
using Bunker.ContentService.Features.Cards.CreateLuggageCard;
using Bunker.ContentService.Features.Cards.GetLuggageCards;
using Bunker.ContentService.Features.Cards.UpdateLuggageCard;
using Bunker.ContentService.Api.Cards.Endpoints.Requests;
using Bunker.ContentService.Api.Cards.Endpoints.Responses;
using Bunker.ContentService.Transfers;

namespace Bunker.ContentService.Api.Cards.Endpoints;

internal static class LuggageCardEndpoints
{
    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<CardResponse.LuggageCard>(StatusCodes.Status200OK)]
    [ProducesResponseType<Microsoft.AspNetCore.Http.HttpResults.ValidationProblem>(StatusCodes.Status400BadRequest)]
    internal static async Task<IResult> CreateLuggageCard(
        [FromBody] CardRequest.Post.LuggageCard request,
        [FromServices] IMessageBus bus,
        [FromServices] IValidator<CardRequest.Post.LuggageCard> validator)
    {
        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await bus.InvokeAsync<CreateLuggageCard.Result>(new CreateLuggageCard(request.Luggage));

        return result switch
        {
            CreateLuggageCard.Result.Success success => TypedResults.Ok(new CardResponse.LuggageCard(success.Card.ToTransferObject())),
            _ => throw new Exception("Unexpected error occurred during luggage card creation."),
        };
    }

    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<CardResponse.LuggageCard>(StatusCodes.Status200OK)]
    [ProducesResponseType<Microsoft.AspNetCore.Http.HttpResults.ValidationProblem>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Microsoft.AspNetCore.Http.HttpResults.NotFound>(StatusCodes.Status404NotFound)]
    internal static async Task<IResult> UpdateLuggageCard(
    [FromRoute] Guid id,
    [FromBody] CardRequest.Put.LuggageCard request,
    [FromServices] IMessageBus bus,
    [FromServices] IValidator<CardRequest.Put.LuggageCard> validator)
    {
        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await bus.InvokeAsync<UpdateLuggageCard.Result>
            (new UpdateLuggageCard(Id: Card.Id.Create(id), Luggage: request.Luggage));

        return result switch
        {
            UpdateLuggageCard.Result.Success success => TypedResults.Ok(new CardResponse.LuggageCard(success.Card.ToTransferObject())),
            Features.Cards.UpdateLuggageCard.UpdateLuggageCard.Result.NotFound => TypedResults.NotFound(),
            _ => throw new Exception("Unexpected error occurred during luggage card updating."),
        };
    }

    [ProducesResponseType<CardResponse.LuggageCards>(StatusCodes.Status200OK)]
    internal static async Task<IResult> GetLuggageCards(
        [FromServices] IMessageBus bus,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 10
    )
    {
        var result = await bus.InvokeAsync<GetLuggageCards.Result>(new GetLuggageCards(Skip: skip, Take: take));

        return result switch
        {
            GetLuggageCards.Result.Success success
            => TypedResults.Ok(
                    new CardResponse.LuggageCards(
                        Total: success.Total,
                        Cards: success.Cards.Select(x => x.ToTransferObject()))),

            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }
}