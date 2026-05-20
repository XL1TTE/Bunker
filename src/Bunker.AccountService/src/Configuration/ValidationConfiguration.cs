
using FluentValidation;

namespace Bunker.AccountService.Api.Configuration;

internal static class ValidationConfiguration
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder IncludeFluentValidation()
        {
            ValidatorOptions.Global.LanguageManager.Enabled = false;
            builder.Services.AddValidatorsFromAssemblyContaining<Validators>(includeInternalTypes: true);
            return builder;
        }
    }
}
