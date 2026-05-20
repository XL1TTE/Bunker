using Microsoft.EntityFrameworkCore;

namespace Bunker.LobbyService.Persistence.Configuration;

internal static class PersistenceConfiguration
{
    internal static IHostApplicationBuilder IncludePersistence(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("lobby-db");
        builder.Services.AddDbContext<LobbyDbContext>(options =>
            options.UseNpgsql(connectionString));
        return builder;
    }

    internal static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LobbyDbContext>();
        await context.Database.EnsureCreatedAsync();
    }
}
