using Microsoft.AspNetCore.Mvc;
using Wolverine;
using Microsoft.AspNetCore.Authorization;
using FluentValidation;
using Bunker.ContentService.Api.BunkerCards.Endpoints.Requests;
using Bunker.ContentService.Api.BunkerCards.Endpoints.Responses;
using Bunker.ContentService.Features.BunkerCards.CreateBunkerCard;
using Bunker.ContentService.Features.BunkerCards.UpdateBunkerCard;
using Bunker.ContentService.Features.BunkerCards.DeleteBunkerCard;
using Bunker.ContentService.Features.BunkerCards.GetBunkerCard;
using Bunker.ContentService.Features.BunkerCards.GetAllBunkerCards;
using Microsoft.AspNetCore.Http;
using Bunker.ContentService.Domain;
using Bunker.ContentService.Transfers;

namespace Bunker.ContentService.Endpoints.BunkerCards;

internal static class BunkerCardEndpoints
{
    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<BunkerCardResponse.Created>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    internal static async Task<IResult> Create(
        [FromBody] BunkerCardRequest.Post.Create request,
        [FromServices] IMessageBus bus,
        [FromServices] IValidator<BunkerCardRequest.Post.Create> validator)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());

        var result = await bus.InvokeAsync<CreateBunkerCard.Result>(
            new CreateBunkerCard(request.Catastrophe, request.SurvivalDuration, request.BunkerEnvironment));

        return result switch
        {
            CreateBunkerCard.Result.Success success => TypedResults.Ok(new BunkerCardResponse.Created(success.Card.ToTransferObject())),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }

    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<BunkerCardResponse.Updated>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    internal static async Task<IResult> Update(
        [FromRoute] Guid id,
        [FromBody] BunkerCardRequest.Put.Update request,
        [FromServices] IMessageBus bus,
        [FromServices] IValidator<BunkerCardRequest.Put.Update> validator)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());

        var result = await bus.InvokeAsync<UpdateBunkerCard.Result>(
            new UpdateBunkerCard(BunkerCard.Id.Create(id), request.Catastrophe, request.SurvivalDuration, request.BunkerEnvironment));

        return result switch
        {
            UpdateBunkerCard.Result.Success success => TypedResults.Ok(new BunkerCardResponse.Updated(success.Card.ToTransferObject())),
            UpdateBunkerCard.Result.NotFound => TypedResults.NotFound(),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }

    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    internal static async Task<IResult> Delete(
        [FromRoute] Guid id,
        [FromServices] IMessageBus bus)
    {
        var result = await bus.InvokeAsync<DeleteBunkerCard.Result>(new DeleteBunkerCard(BunkerCard.Id.Create(id)));

        return result switch
        {
            DeleteBunkerCard.Result.Success => TypedResults.NoContent(),
            DeleteBunkerCard.Result.NotFound => TypedResults.NotFound(),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }

    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<BunkerCardResponse.Single>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    internal static async Task<IResult> GetById(
        [FromRoute] Guid id,
        [FromServices] IMessageBus bus)
    {
        var result = await bus.InvokeAsync<GetBunkerCard.Result>(new GetBunkerCard(BunkerCard.Id.Create(id)));

        return result switch
        {
            GetBunkerCard.Result.Success success => TypedResults.Ok(new BunkerCardResponse.Single(success.Card.ToTransferObject())),
            GetBunkerCard.Result.NotFound => TypedResults.NotFound(),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }

    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<BunkerCardResponse.All>(StatusCodes.Status200OK)]
    internal static async Task<IResult> GetAll(
        [FromServices] IMessageBus bus)
    {
        var result = await bus.InvokeAsync<GetAllBunkerCards.Result>(new GetAllBunkerCards());

        return result switch
        {
            GetAllBunkerCards.Result.Success success => TypedResults.Ok(new BunkerCardResponse.All(success.Cards.Select(x => x.ToTransferObject()))),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }
}