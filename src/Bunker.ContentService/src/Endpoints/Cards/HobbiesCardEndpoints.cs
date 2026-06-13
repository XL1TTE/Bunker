using Wolverine;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Bunker.ContentService.Domain;
using Bunker.ContentService.Features.Cards.CreateHobbiesCard;
using Bunker.ContentService.Features.Cards.GetHobbiesCards;
using Bunker.ContentService.Features.Cards.UpdateHobbiesCard;
using Bunker.ContentService.Api.Cards.Endpoints.Requests;
using Bunker.ContentService.Api.Cards.Endpoints.Responses;
using Bunker.ContentService.Transfers;

namespace Bunker.ContentService.Api.Cards.Endpoints;

internal static class HobbiesCardEndpoints
{
    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<CardResponse.HobbiesCard>(StatusCodes.Status200OK)]
    [ProducesResponseType<Microsoft.AspNetCore.Http.HttpResults.ValidationProblem>(StatusCodes.Status400BadRequest)]
    internal static async Task<IResult> CreateHobbiesCard(
        [FromBody] CardRequest.Post.HobbiesCard request,
        [FromServices] IMessageBus bus,
        [FromServices] IValidator<CardRequest.Post.HobbiesCard> validator)
    {
        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await bus.InvokeAsync<CreateHobbiesCard.Result>(new CreateHobbiesCard(request.Hobbies));

        return result switch
        {
            CreateHobbiesCard.Result.Success success => TypedResults.Ok(new CardResponse.HobbiesCard(success.Card.ToTransferObject())),
            _ => throw new Exception("Unexpected error occurred during hobbies card creation."),
        };
    }

    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<CardResponse.HobbiesCard>(StatusCodes.Status200OK)]
    [ProducesResponseType<Microsoft.AspNetCore.Http.HttpResults.ValidationProblem>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Microsoft.AspNetCore.Http.HttpResults.NotFound>(StatusCodes.Status404NotFound)]
    internal static async Task<IResult> UpdateHobbiesCard(
    [FromRoute] Guid id,
    [FromBody] CardRequest.Put.HobbiesCard request,
    [FromServices] IMessageBus bus,
    [FromServices] IValidator<CardRequest.Put.HobbiesCard> validator)
    {
        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await bus.InvokeAsync<UpdateHobbiesCard.Result>
            (new UpdateHobbiesCard(Id: Card.Id.Create(id), Hobbies: request.Hobbies));

        return result switch
        {
            UpdateHobbiesCard.Result.Success success => TypedResults.Ok(new CardResponse.HobbiesCard(success.Card.ToTransferObject())),
            Features.Cards.UpdateHobbiesCard.UpdateHobbiesCard.Result.NotFound => TypedResults.NotFound(),
            _ => throw new Exception("Unexpected error occurred during hobbies card updating."),
        };
    }

    [ProducesResponseType<CardResponse.HobbiesCards>(StatusCodes.Status200OK)]
    internal static async Task<IResult> GetHobbiesCards([FromServices] IMessageBus bus)
    {
        var result = await bus.InvokeAsync<GetHobbiesCards.Result>(new GetHobbiesCards());

        return result switch
        {
            GetHobbiesCards.Result.Success success
            => TypedResults.Ok(
                    new CardResponse.HobbiesCards(
                        Total: success.Total,
                        Cards: success.Cards.Select(x => x.ToTransferObject()))),

            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }
}
