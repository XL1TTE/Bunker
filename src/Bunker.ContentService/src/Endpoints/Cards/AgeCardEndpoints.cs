using Wolverine;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Bunker.ContentService.Domain;
using Bunker.ContentService.Features.Cards.CreateAgeCard;
using Bunker.ContentService.Features.Cards.GetAgeCards;
using Bunker.ContentService.Features.Cards.UpdateAgeCard;
using Bunker.ContentService.Api.Cards.Endpoints.Requests;
using Bunker.ContentService.Api.Cards.Endpoints.Responses;
using Bunker.ContentService.Transfers;

namespace Bunker.ContentService.Api.Cards.Endpoints;

internal static class AgeCardEndpoints
{
    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<CardResponse.AgeCard>(StatusCodes.Status200OK)]
    [ProducesResponseType<Microsoft.AspNetCore.Http.HttpResults.ValidationProblem>(StatusCodes.Status400BadRequest)]
    internal static async Task<IResult> CreateAgeCard(
        [FromBody] CardRequest.Post.AgeCard request,
        [FromServices] IMessageBus bus,
        [FromServices] IValidator<CardRequest.Post.AgeCard> validator)
    {
        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await bus.InvokeAsync<CreateAgeCard.Result>(new CreateAgeCard(request.Age));

        return result switch
        {
            CreateAgeCard.Result.Success success => TypedResults.Ok(new CardResponse.AgeCard(success.Card.ToTransferObject())),
            _ => throw new Exception("Unexpected error occurred during age card creation."),
        };
    }

    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<CardResponse.AgeCard>(StatusCodes.Status200OK)]
    [ProducesResponseType<Microsoft.AspNetCore.Http.HttpResults.ValidationProblem>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Microsoft.AspNetCore.Http.HttpResults.NotFound>(StatusCodes.Status404NotFound)]
    internal static async Task<IResult> UpdateAgeCard(
    [FromRoute] Guid id,
    [FromBody] CardRequest.Put.AgeCard request,
    [FromServices] IMessageBus bus,
    [FromServices] IValidator<CardRequest.Put.AgeCard> validator)
    {
        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await bus.InvokeAsync<UpdateAgeCard.Result>
            (new UpdateAgeCard(Id: Card.Id.Create(id), Age: request.Age));

        return result switch
        {
            UpdateAgeCard.Result.Success success => TypedResults.Ok(new CardResponse.AgeCard(success.Card.ToTransferObject())),
            Features.Cards.UpdateAgeCard.UpdateAgeCard.Result.NotFound => TypedResults.NotFound(),
            _ => throw new Exception("Unexpected error occurred during age card updating."),
        };
    }

    [ProducesResponseType<CardResponse.AgeCards>(StatusCodes.Status200OK)]
    internal static async Task<IResult> GetAgeCards(
        [FromServices] IMessageBus bus,
        [FromQuery] int Skip = 0,
        [FromQuery] int Take = 10)
    {
        var result = await bus.InvokeAsync<GetAgeCards.Result>(new GetAgeCards(Skip, Take));

        return result switch
        {
            GetAgeCards.Result.Success success
            => TypedResults.Ok(
                    new CardResponse.AgeCards(
                        Total: success.Total,
                        Cards: success.Cards.Select(x => x.ToTransferObject()))),

            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }
}
