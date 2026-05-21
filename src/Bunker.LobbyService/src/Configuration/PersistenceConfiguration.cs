using Microsoft.EntityFrameworkCore;

namespace Bunker.LobbyService.Persistence.Configuration;

internal static class PersistenceConfiguration
{
    internal static IHostApplicationBuilder IncludePersistence(this IHostApplicationBuilder builder)
    {
        var dbConnection = builder.Configuration.GetConnectionString("lobby-db");
        var accountsReplicaDbConnection = builder.Configuration.GetConnectionString("lobby-accounts-replica-db");

        builder.Services.AddDbContext<LobbyDbContext>(options =>
            options.UseNpgsql(dbConnection));

        builder.Services.AddDbContext<AccountsDbContext>(options =>
            options.UseNpgsql(accountsReplicaDbConnection));

        return builder;
    }

    internal static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<LobbyDbContext>();
        await db.Database.EnsureCreatedAsync();

        var accountReplica = scope.ServiceProvider.GetRequiredService<AccountsDbContext>();
        await accountReplica.Database.EnsureCreatedAsync();
    }
}
