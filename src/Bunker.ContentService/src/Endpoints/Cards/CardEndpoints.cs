using Microsoft.AspNetCore.Mvc;
using Wolverine;
using Microsoft.AspNetCore.Authorization;
using Bunker.ContentService.Domain;
using Bunker.ContentService.Features.Cards.DeleteCard;
using Bunker.ContentService.Features.Cards.GetCardById;
using Bunker.ContentService.Api.Cards.Endpoints.Responses;

namespace Bunker.ContentService.Api.Endpoints.Cards;

internal static class CardEndpoints
{
    #region Common
    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    internal static async Task<IResult> Delete(
        [FromRoute] Guid id,
        [FromServices] IMessageBus bus)
    {
        var result = await bus.InvokeAsync<DeleteCard.Result>(new DeleteCard(Card.Id.Create(id)));

        return result switch
        {
            DeleteCard.Result.Success => TypedResults.NoContent(),
            DeleteCard.Result.NotFound => TypedResults.NotFound(),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }

    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<Domain.Card>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    internal static async Task<IResult> GetById(
        [FromRoute] Guid id,
        [FromServices] IMessageBus bus)
    {
        var result = await bus.InvokeAsync<GetCardById.Result>(new GetCardById(Card.Id.Create(id)));

        return result switch
        {
            GetCardById.Result.Success success => TypedResults.Ok(success.Card),
            GetCardById.Result.NotFound => TypedResults.NotFound(),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }
    #endregion
}
