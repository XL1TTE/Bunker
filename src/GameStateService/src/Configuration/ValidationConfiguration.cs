using GameStateService.Validation;
using FluentValidation;
using MicroElements.AspNetCore.OpenApi.FluentValidation;

namespace GameStateService.Configuration;

internal static class ValidationConfiguration
{
    internal static IHostApplicationBuilder IncludeFluentValidation(this IHostApplicationBuilder builder)
    {
        ValidatorOptions.Global.LanguageManager.Enabled = false;
        builder.Services.AddValidatorsFromAssemblyContaining<ValidationMarker>(includeInternalTypes: true);
        return builder;
    }
}
