using Bunker.LobbyService.Persistence.Abstractions;
using Bunker.LobbyService.Persistence.Queries;
using Microsoft.EntityFrameworkCore;

namespace Bunker.LobbyService.Persistence.Configuration;

internal static class PersistenceConfiguration
{
    internal static IHostApplicationBuilder IncludePersistence(this IHostApplicationBuilder builder)
    {
        var dbConnection = builder.Configuration.GetConnectionString("lobby-db");
        var accountsCacheConnection = builder.Configuration.GetConnectionString("lobby-accounts-cache");

        builder.Services.AddDbContext<LobbyDbContext>(options =>
            options.UseNpgsql(dbConnection));

        builder.Services.AddDbContext<AccountsDbContext>(options =>
            options.UseNpgsql(accountsCacheConnection));

        builder.Services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<LobbyDbContext>());
        builder.Services.AddScoped<ILobbyQueries, LobbyQueries>();

        return builder;
    }

    internal static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<LobbyDbContext>();
        await db.Database.EnsureCreatedAsync();

        var accountCache = scope.ServiceProvider.GetRequiredService<AccountsDbContext>();
        await accountCache.Database.EnsureCreatedAsync();
    }
}
