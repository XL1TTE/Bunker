using Microsoft.AspNetCore.Mvc;
using Wolverine;
using Microsoft.AspNetCore.Authorization;
using FluentValidation;
using Bunker.ContentService.Api.CardPacks.Endpoints.Requests;
using Bunker.ContentService.Api.CardPacks.Endpoints.Responses;
using Bunker.ContentService.Features.CardPacks.CreateCardPack;
using Bunker.ContentService.Features.CardPacks.UpdateCardPack;
using Bunker.ContentService.Features.CardPacks.DeleteCardPack;
using Bunker.ContentService.Features.CardPacks.GetCardPack;
using Bunker.ContentService.Features.CardPacks.GetAllCardPacks;
using Bunker.ContentService.Features.CardPacks.AddCardToPack;
using Bunker.ContentService.Features.CardPacks.RemoveCardFromPack;
using Microsoft.AspNetCore.Http;
using Bunker.ContentService.Domain;
using Bunker.ContentService.Transfers;

namespace Bunker.ContentService.Endpoints.CardPacks;

internal static class CardPackEndpoints
{
    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<CardPackResponse.Created>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    internal static async Task<IResult> Create(
        [FromBody] CardPackRequest.Post.Create request,
        [FromServices] IMessageBus bus,
        [FromServices] IValidator<CardPackRequest.Post.Create> validator)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());

        var result = await bus.InvokeAsync<CreateCardPack.Result>(
            new CreateCardPack(request.Title, request.Description, request.CardIds));

        return result switch
        {
            CreateCardPack.Result.Success success => TypedResults.Ok(new CardPackResponse.Created(success.Pack.ToTransferObject())),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }

    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<CardPackResponse.Updated>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    internal static async Task<IResult> Update(
        [FromRoute] Guid id,
        [FromBody] CardPackRequest.Put.Update request,
        [FromServices] IMessageBus bus,
        [FromServices] IValidator<CardPackRequest.Put.Update> validator)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());

        var result = await bus.InvokeAsync<UpdateCardPack.Result>(
            new UpdateCardPack(CardPack.Id.Create(id), request.Title, request.Description, request.CardIds));

        return result switch
        {
            UpdateCardPack.Result.Success success => TypedResults.Ok(new CardPackResponse.Updated(success.Pack.ToTransferObject())),
            UpdateCardPack.Result.NotFound => TypedResults.NotFound(),
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
        var result = await bus.InvokeAsync<DeleteCardPack.Result>(new DeleteCardPack(CardPack.Id.Create(id)));

        return result switch
        {
            DeleteCardPack.Result.Success => TypedResults.NoContent(),
            DeleteCardPack.Result.NotFound => TypedResults.NotFound(),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }

    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<CardPackResponse.Single>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    internal static async Task<IResult> GetById(
        [FromRoute] Guid id,
        [FromServices] IMessageBus bus)
    {
        var result = await bus.InvokeAsync<GetCardPack.Result>(new GetCardPack(CardPack.Id.Create(id)));

        return result switch
        {
            GetCardPack.Result.Success success => TypedResults.Ok(new CardPackResponse.Single(success.Pack.ToTransferObject())),
            GetCardPack.Result.NotFound => TypedResults.NotFound(),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }

    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<CardPackResponse.All>(StatusCodes.Status200OK)]
    internal static async Task<IResult> GetAll(
        [FromServices] IMessageBus bus)
    {
        var result = await bus.InvokeAsync<GetAllCardPacks.Result>(new GetAllCardPacks());

        return result switch
        {
            GetAllCardPacks.Result.Success success => TypedResults.Ok(new CardPackResponse.All(success.Packs.Select(x => x.ToTransferObject()))),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }

    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<CardPackResponse.Updated>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    internal static async Task<IResult> AddCard(
        [FromRoute] Guid id,
        [FromRoute] Guid cardId,
        [FromServices] IMessageBus bus)
    {
        var result = await bus.InvokeAsync<AddCardToPack.Result>(new AddCardToPack(CardPack.Id.Create(id), Card.Id.Create(cardId)));

        return result switch
        {
            AddCardToPack.Result.Success success => TypedResults.Ok(new CardPackResponse.Updated(success.Pack.ToTransferObject())),
            AddCardToPack.Result.NotFound => TypedResults.NotFound(),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }

    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<CardPackResponse.Updated>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    internal static async Task<IResult> RemoveCard(
        [FromRoute] Guid id,
        [FromRoute] Guid cardId,
        [FromServices] IMessageBus bus)
    {
        var result = await bus.InvokeAsync<RemoveCardFromPack.Result>(new RemoveCardFromPack(CardPack.Id.Create(id), Card.Id.Create(cardId)));

        return result switch
        {
            RemoveCardFromPack.Result.Success success => TypedResults.Ok(new CardPackResponse.Updated(success.Pack.ToTransferObject())),
            RemoveCardFromPack.Result.NotFound => TypedResults.NotFound(),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }
}
