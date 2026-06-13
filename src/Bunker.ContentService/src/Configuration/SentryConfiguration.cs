using Sentry.OpenTelemetry;


namespace Bunker.ContentService.Api.Configuration;

internal static class SentryConfiguration
{
    public static void ConfigureSentry(this WebApplicationBuilder builder)
    {      
        builder.WebHost.UseSentry(options =>
        {
            options.Dsn = builder.Configuration["Sentry:Dsn"];
            options.SendDefaultPii = true;
            options.SampleRate = 1.0f;
            options.TracesSampleRate = 1.0;
            options.EnableLogs = true;
            options.UseOpenTelemetry();
            options.DisableSentryHttpMessageHandler = true;
        });
    }
}


