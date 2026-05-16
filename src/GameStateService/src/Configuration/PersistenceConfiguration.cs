using GameStateService.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GameStateService.Configuration;

internal static class PersistenceConfiguration
{
    internal static IHostApplicationBuilder IncludePersistence(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("game-state-db");
        
        builder.Services.AddDbContext<GameStateDbContext>(options =>
            options.UseNpgsql(connectionString));
            
        return builder;
    }

    internal static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GameStateDbContext>();
        await context.Database.EnsureCreatedAsync();
    }
}
