using Microsoft.AspNetCore.Mvc;
using Wolverine;
using Microsoft.AspNetCore.Authorization;
using FluentValidation;
using Bunker.ContentService.Features.CardPacks;
using Bunker.ContentService.Messages;
using Microsoft.AspNetCore.Http;

namespace Bunker.ContentService.Endpoints.CardPacks;

internal static class CardPackEndpoints
{
    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<CardPackUpdated>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    internal static async Task<IResult> Create(
        [FromBody] CreateCardPack cmd,
        [FromServices] IMessageBus bus,
        [FromServices] IValidator<CreateCardPack> validator)
    {
        var validationResult = await validator.ValidateAsync(cmd);
        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());

        var result = await bus.InvokeAsync<CardPackResult>(cmd);

        return result switch
        {
            CardPackUpdatedSuccess success => TypedResults.Ok(success.Updated),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }

    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<CardPackUpdated>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    internal static async Task<IResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateCardPack cmd,
        [FromServices] IMessageBus bus,
        [FromServices] IValidator<UpdateCardPack> validator)
    {
        cmd = cmd with { Id = id };
        var validationResult = await validator.ValidateAsync(cmd);
        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());

        var result = await bus.InvokeAsync<CardPackResult>(cmd);

        return result switch
        {
            CardPackUpdatedSuccess success => TypedResults.Ok(success.Updated),
            CardPackNotFound => TypedResults.NotFound(),
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
        var result = await bus.InvokeAsync<CardPackResult>(new DeleteCardPack(id));

        return result switch
        {
            CardPackDeletedSuccess => TypedResults.NoContent(),
            CardPackNotFound => TypedResults.NotFound(),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }

    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<Domain.CardPack>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    internal static async Task<IResult> GetById(
        [FromRoute] Guid id,
        [FromServices] IMessageBus bus)
    {
        var result = await bus.InvokeAsync<CardPackResult>(new GetCardPack(id));

        return result switch
        {
            CardPackSuccess success => TypedResults.Ok(success.Pack),
            CardPackNotFound => TypedResults.NotFound(),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }

    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<IEnumerable<Domain.CardPack>>(StatusCodes.Status200OK)]
    internal static async Task<IResult> GetAll(
        [FromServices] IMessageBus bus)
    {
        var result = await bus.InvokeAsync<CardPackResult>(new GetAllCardPacks());

        return result switch
        {
            CardPacksSuccess success => TypedResults.Ok(success.Packs),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }

    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<CardPackUpdated>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    internal static async Task<IResult> AddCard(
        [FromRoute] Guid id,
        [FromRoute] Guid cardId,
        [FromServices] IMessageBus bus,
        [FromServices] IValidator<AddCardToPack> validator)
    {
        var cmd = new AddCardToPack(id, cardId);
        var validationResult = await validator.ValidateAsync(cmd);
        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());

        var result = await bus.InvokeAsync<CardPackResult>(cmd);

        return result switch
        {
            CardPackUpdatedSuccess success => TypedResults.Ok(success.Updated),
            CardPackNotFound => TypedResults.NotFound(),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }

    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<CardPackUpdated>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    internal static async Task<IResult> RemoveCard(
        [FromRoute] Guid id,
        [FromRoute] Guid cardId,
        [FromServices] IMessageBus bus,
        [FromServices] IValidator<RemoveCardFromPack> validator)
    {
        var cmd = new RemoveCardFromPack(id, cardId);
        var validationResult = await validator.ValidateAsync(cmd);
        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());

        var result = await bus.InvokeAsync<CardPackResult>(cmd);

        return result switch
        {
            CardPackUpdatedSuccess success => TypedResults.Ok(success.Updated),
            CardPackNotFound => TypedResults.NotFound(),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }
}
