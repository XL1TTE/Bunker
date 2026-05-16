using PlayerService.Validation;
using FluentValidation;
using MicroElements.AspNetCore.OpenApi.FluentValidation;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace PlayerService.Configuration;

internal static class ValidationConfiguration
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder IncludeFluentValidation()
        {
            ValidatorOptions.Global.LanguageManager.Enabled = false;
            builder.Services.AddValidatorsFromAssemblyContaining<ValidationMarker>(includeInternalTypes: true);
            return builder;
        }
    }
}
