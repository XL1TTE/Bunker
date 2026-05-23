using Wolverine;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Bunker.ContentService.Domain;
using Bunker.ContentService.Features.Cards.CreateFactCard;
using Bunker.ContentService.Features.Cards.GetFactCards;
using Bunker.ContentService.Features.Cards.UpdateFactCard;
using Bunker.ContentService.Api.Cards.Endpoints.Requests;
using Bunker.ContentService.Api.Cards.Endpoints.Responses;

namespace Bunker.ContentService.Api.Cards.Endpoints;

internal static class FactCardEndpoints
{
    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<CardResponse.FactCard>(StatusCodes.Status200OK)]
    [ProducesResponseType<Microsoft.AspNetCore.Http.HttpResults.ValidationProblem>(StatusCodes.Status400BadRequest)]
    internal static async Task<IResult> CreateFact(
        [FromBody] CardRequest.Post.FactCard request,
        [FromServices] IMessageBus bus,
        [FromServices] IValidator<CardRequest.Post.FactCard> validator)
    {
        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await bus.InvokeAsync<CreateFactCard.Result>(new CreateFactCard(request.Fact));

        return result switch
        {
            CreateFactCard.Result.Success success => TypedResults.Ok(success.Card),
            _ => throw new Exception("Unexpected error occurred during fact card creation."),
        };
    }

    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<CardResponse.FactCard>(StatusCodes.Status200OK)]
    [ProducesResponseType<Microsoft.AspNetCore.Http.HttpResults.ValidationProblem>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Microsoft.AspNetCore.Http.HttpResults.NotFound>(StatusCodes.Status404NotFound)]
    internal static async Task<IResult> UpdateFact(
    [FromRoute] Guid id,
    [FromBody] CardRequest.Put.FactCard request,
    [FromServices] IMessageBus bus,
    [FromServices] IValidator<CardRequest.Put.FactCard> validator)
    {
        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await bus.InvokeAsync<UpdateFactCard.Result>
            (new UpdateFactCard(Id: Card.Id.Create(id), Fact: request.Fact));

        return result switch
        {
            UpdateFactCard.Result.Success success => TypedResults.Ok(success.Card),
            UpdateFactCard.Result.NotFound => TypedResults.NotFound(),
            _ => throw new Exception("Unexpected error occurred during fact card updating."),
        };
    }

    [ProducesResponseType<IEnumerable<CardResponse.FactCard>>(StatusCodes.Status200OK)]
    internal static async Task<IResult> GetFactCards([FromServices] IMessageBus bus)
    {
        var result = await bus.InvokeAsync<GetFactCards.Result>(new GetFactCards());

        return result switch
        {
            GetFactCards.Result.Success success => TypedResults.Ok(success.Cards),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }
}