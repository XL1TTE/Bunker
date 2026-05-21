
using Serilog;

namespace Bunker.LobbyService.Api.Configuration;

internal static class WolverineConfiguration
{
    internal static IHostApplicationBuilder ConfigureLogging(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSerilog((_, config) =>
        {
            config.ReadFrom.Configuration(builder.Configuration);
        });
        return builder;
    }
}
