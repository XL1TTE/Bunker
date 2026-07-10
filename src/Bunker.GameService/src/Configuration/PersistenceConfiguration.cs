using Bunker.GameService.Persistence.Contracts;
using Bunker.GameService.Persistence.Contracts.Queries;
using Bunker.GameService.Persistence.Queries;
using Microsoft.EntityFrameworkCore;

namespace Bunker.GameService.Persistence;

internal static class PersistenceConfiguration
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder IncludePersistence()
        {
            var connectionString = builder.Configuration.GetConnectionString("game-state-db");

            builder.Services.AddNpgsql<GameDbContext>(connectionString);

            builder.Services.AddScoped<IUnitOfWork, GameDbContext>(provider => provider.GetRequiredService<GameDbContext>());
            builder.Services.AddScoped<IGameQueries, GameQueries>();

            return builder;
        }
    }

    extension(WebApplication app)
    {
        public async Task InitializeDatabaseAsync()
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GameDbContext>();
            await context.Database.EnsureCreatedAsync();
        }
    }
}