using Wolverine;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Bunker.ContentService.Domain;
using Bunker.ContentService.Features.Cards.CreateProfessionCard;
using Bunker.ContentService.Features.Cards.GetProfessionCards;
using Bunker.ContentService.Features.Cards.UpdateProfessionCard;
using Bunker.ContentService.Api.Cards.Endpoints.Requests;
using Bunker.ContentService.Api.Cards.Endpoints.Responses;
using Bunker.ContentService.Transfers;

namespace Bunker.ContentService.Api.Cards.Endpoints;

internal static class ProfessionCardEndpoints
{
    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<CardResponse.ProfessionCard>(StatusCodes.Status200OK)]
    [ProducesResponseType<Microsoft.AspNetCore.Http.HttpResults.ValidationProblem>(StatusCodes.Status400BadRequest)]
    internal static async Task<IResult> CreateProfession(
        [FromBody] CardRequest.Post.ProfessionCard request,
        [FromServices] IMessageBus bus,
        [FromServices] IValidator<CardRequest.Post.ProfessionCard> validator)
    {
        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await bus.InvokeAsync<CreateProfessionCard.Result>(new CreateProfessionCard(request.Profession));

        return result switch
        {
            CreateProfessionCard.Result.Success success => TypedResults.Ok(new CardResponse.ProfessionCard(success.Card.ToTransferObject())),
            _ => throw new Exception("Unexpected error occurred during profession card creation."),
        };
    }

    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<CardResponse.ProfessionCard>(StatusCodes.Status200OK)]
    [ProducesResponseType<Microsoft.AspNetCore.Http.HttpResults.ValidationProblem>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Microsoft.AspNetCore.Http.HttpResults.NotFound>(StatusCodes.Status404NotFound)]
    internal static async Task<IResult> UpdateProfession(
    [FromRoute] Guid id,
    [FromBody] CardRequest.Put.ProfessionCard request,
    [FromServices] IMessageBus bus,
    [FromServices] IValidator<CardRequest.Put.ProfessionCard> validator)
    {
        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await bus.InvokeAsync<UpdateProfessionCard.Result>
            (new UpdateProfessionCard(Id: Card.Id.Create(id), Profession: request.Profession));

        return result switch
        {
            UpdateProfessionCard.Result.Success success => TypedResults.Ok(new CardResponse.ProfessionCard(success.Card.ToTransferObject())),
            UpdateProfessionCard.Result.NotFound => TypedResults.NotFound(),
            _ => throw new Exception("Unexpected error occurred during profession card updating."),
        };
    }

    [ProducesResponseType<CardResponse.ProfessionCards>(StatusCodes.Status200OK)]
    internal static async Task<IResult> GetProfessionCards(
        [FromServices] IMessageBus bus,
        [FromQuery] int Skip = 0,
        [FromQuery] int Take = 10)
    {
        var result = await bus.InvokeAsync<GetProfessionCards.Result>(new GetProfessionCards(Skip: Skip, Take: Take));

        return result switch
        {
            GetProfessionCards.Result.Success success => TypedResults.Ok(
                    new CardResponse.ProfessionCards(
                        Total: success.Total,
                        Cards: success.Cards.Select(x => x.ToTransferObject()))),

            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }
}
