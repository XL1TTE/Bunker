using Serilog;

namespace Bunker.GameService.Api.Configuration;

internal static class LoggingConfiguration
{
    extension(IHostApplicationBuilder builder)
    {
        internal IHostApplicationBuilder ConfigureLogging()
        {
            builder.Services.AddSerilog((context, config) => config.ReadFrom.Configuration(builder.Configuration));
            return builder;
        }
    }

    extension(IApplicationBuilder builder)
    {
        internal IApplicationBuilder ConfigureLogging()
        {
            builder.UseSerilogRequestLogging();
            return builder;
        }
    }
}