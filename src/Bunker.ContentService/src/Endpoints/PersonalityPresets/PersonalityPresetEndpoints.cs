using Microsoft.AspNetCore.Mvc;
using Wolverine;
using Microsoft.AspNetCore.Authorization;
using FluentValidation;
using Bunker.ContentService.Features.PersonalityPresets;
using Bunker.ContentService.Messages;
using Microsoft.AspNetCore.Http;

namespace Bunker.ContentService.Endpoints.PersonalityPresets;

internal static class PersonalityPresetEndpoints
{
    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<PersonalityPresetUpdated>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    internal static async Task<IResult> Create(
        [FromBody] CreatePersonalityPreset cmd,
        [FromServices] IMessageBus bus,
        [FromServices] IValidator<CreatePersonalityPreset> validator)
    {
        var validationResult = await validator.ValidateAsync(cmd);
        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());

        var result = await bus.InvokeAsync<PersonalityPresetResult>(cmd);

        return result switch
        {
            PersonalityPresetUpdatedSuccess success => TypedResults.Ok(success.Updated),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }

    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<PersonalityPresetUpdated>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    internal static async Task<IResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdatePersonalityPreset cmd,
        [FromServices] IMessageBus bus,
        [FromServices] IValidator<UpdatePersonalityPreset> validator)
    {
        cmd = cmd with { Id = id };
        var validationResult = await validator.ValidateAsync(cmd);
        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());

        var result = await bus.InvokeAsync<PersonalityPresetResult>(cmd);

        return result switch
        {
            PersonalityPresetUpdatedSuccess success => TypedResults.Ok(success.Updated),
            PersonalityPresetNotFound => TypedResults.NotFound(),
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
        var result = await bus.InvokeAsync<PersonalityPresetResult>(new DeletePersonalityPreset(id));

        return result switch
        {
            PersonalityPresetDeletedSuccess => TypedResults.NoContent(),
            PersonalityPresetNotFound => TypedResults.NotFound(),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }

    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<Domain.PersonalityPreset>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    internal static async Task<IResult> GetById(
        [FromRoute] Guid id,
        [FromServices] IMessageBus bus)
    {
        var result = await bus.InvokeAsync<PersonalityPresetResult>(new GetPersonalityPreset(id));

        return result switch
        {
            PersonalityPresetSuccess success => TypedResults.Ok(success.Preset),
            PersonalityPresetNotFound => TypedResults.NotFound(),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }

    [Authorize(Roles = "content-service.admin")]
    [ProducesResponseType<IEnumerable<Domain.PersonalityPreset>>(StatusCodes.Status200OK)]
    internal static async Task<IResult> GetAll(
        [FromServices] IMessageBus bus)
    {
        var result = await bus.InvokeAsync<PersonalityPresetResult>(new GetAllPersonalityPresets());

        return result switch
        {
            PersonalityPresetsSuccess success => TypedResults.Ok(success.Presets),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }
}
