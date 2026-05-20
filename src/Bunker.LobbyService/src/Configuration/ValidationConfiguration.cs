using LobbyService.Validation;
using FluentValidation;

namespace Bunker.LobbyService.Validation.Configuration;

internal static class ValidationConfiguration
{
    internal static IHostApplicationBuilder IncludeFluentValidation(this IHostApplicationBuilder builder)
    {
        ValidatorOptions.Global.LanguageManager.Enabled = false;
        builder.Services.AddValidatorsFromAssemblyContaining<ValidationMarker>(includeInternalTypes: true);
        return builder;
    }
}
