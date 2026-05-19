
namespace Bunker.ContentService.Validation.Configuration;

internal static class ValidationConfiguration
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder IncludeFluentValidation()
        {
            // ValidatorOptions.Global.LanguageManager.Enabled = false;
            // builder.Services.AddValidatorsFromAssemblyContaining<ValidationMarker>(includeInternalTypes: true);
            return builder;
        }
    }
}
