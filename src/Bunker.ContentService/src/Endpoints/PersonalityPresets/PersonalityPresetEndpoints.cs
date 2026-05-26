using Microsoft.AspNetCore.Mvc;
using Wolverine;
using Microsoft.AspNetCore.Authorization;
using FluentValidation;
using Bunker.ContentService.Api.PersonalityPresets.Endpoints.Requests;
using Bunker.ContentService.Api.PersonalityPresets.Endpoints.Responses;
using Bunker.ContentService.Features.PersonalityPresets.CreatePersonalityPreset;
using Bunker.ContentService.Features.PersonalityPresets.UpdatePersonalityPreset;
using Bunker.ContentService.Features.PersonalityPresets.DeletePersonalityPreset;
using Bunker.ContentService.Features.PersonalityPresets.GetPersonalityPreset;
using Bunker.ContentService.Features.PersonalityPresets.GetAllPersonalityPresets;
using Microsoft.AspNetCore.Http;
using Bunker.ContentService.Domain;
using Bunker.ContentService.Transfers;

namespace Bunker.ContentService.Endpoints.PersonalityPresets;

internal static class PersonalityPresetEndpoints
{
    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<PersonalityPresetResponse.Created>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    internal static async Task<IResult> Create(
        [FromBody] PersonalityPresetRequest.Post.Create request,
        [FromServices] IMessageBus bus,
        [FromServices] IValidator<PersonalityPresetRequest.Post.Create> validator)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());

        var result = await bus.InvokeAsync<CreatePersonalityPreset.Result>(
            new CreatePersonalityPreset(request.Title, request.Description));

        return result switch
        {
            CreatePersonalityPreset.Result.Success success => TypedResults.Ok(new PersonalityPresetResponse.Created(success.Preset.ToTransferObject())),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }

    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<PersonalityPresetResponse.Updated>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    internal static async Task<IResult> Update(
        [FromRoute] Guid id,
        [FromBody] PersonalityPresetRequest.Put.Update request,
        [FromServices] IMessageBus bus,
        [FromServices] IValidator<PersonalityPresetRequest.Put.Update> validator)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());

        var result = await bus.InvokeAsync<UpdatePersonalityPreset.Result>(
            new UpdatePersonalityPreset(PersonalityPreset.Id.Create(id), request.Title, request.Description));

        return result switch
        {
            UpdatePersonalityPreset.Result.Success success => TypedResults.Ok(new PersonalityPresetResponse.Updated(success.Preset.ToTransferObject())),
            UpdatePersonalityPreset.Result.NotFound => TypedResults.NotFound(),
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
        var result = await bus.InvokeAsync<DeletePersonalityPreset.Result>(new DeletePersonalityPreset(PersonalityPreset.Id.Create(id)));

        return result switch
        {
            DeletePersonalityPreset.Result.Success => TypedResults.NoContent(),
            DeletePersonalityPreset.Result.NotFound => TypedResults.NotFound(),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }

    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<PersonalityPresetResponse.Single>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    internal static async Task<IResult> GetById(
        [FromRoute] Guid id,
        [FromServices] IMessageBus bus)
    {
        var result = await bus.InvokeAsync<GetPersonalityPreset.Result>(new GetPersonalityPreset(PersonalityPreset.Id.Create(id)));

        return result switch
        {
            GetPersonalityPreset.Result.Success success => TypedResults.Ok(new PersonalityPresetResponse.Single(success.Preset.ToTransferObject())),
            GetPersonalityPreset.Result.NotFound => TypedResults.NotFound(),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }

    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<PersonalityPresetResponse.All>(StatusCodes.Status200OK)]
    internal static async Task<IResult> GetAll(
        [FromServices] IMessageBus bus)
    {
        var result = await bus.InvokeAsync<GetAllPersonalityPresets.Result>(new GetAllPersonalityPresets());

        return result switch
        {
            GetAllPersonalityPresets.Result.Success success => TypedResults.Ok(new PersonalityPresetResponse.All(success.Presets.Select(x => x.ToTransferObject()))),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }
}
