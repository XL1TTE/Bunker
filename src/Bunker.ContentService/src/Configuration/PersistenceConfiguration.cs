using Bunker.ContentService.Persistence.Contracts;
using Bunker.ContentService.Persistence.Queries;
using Microsoft.EntityFrameworkCore;

namespace Bunker.ContentService.Persistence;

internal static class PersistenceConfiguration
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder IncludePersistence()
        {
            var connectionString = builder.Configuration.GetConnectionString("content-service-db");

            builder.Services.AddNpgsql<ContentDbContext>(connectionString);

            builder.Services.AddScoped<IUnitOfWork, ContentDbContext>(provider => provider.GetRequiredService<ContentDbContext>());
            builder.Services.AddScoped<ICardQueries, DbContextCardQueries>();
            builder.Services.AddScoped<ICardPackQueries, DbContextCardPackQueries>();
            builder.Services.AddScoped<IPersonalityPresetQueries, DbContextPersonalityPresetQueries>();
            builder.Services.AddScoped<IHydrationQueries, DbContextHydrationQueries>();

            return builder;
        }
    }

    extension(WebApplication app)
    {
        public async Task InitializeDatabaseAsync()
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ContentDbContext>();
            await context.Database.EnsureCreatedAsync();
        }
    }
}
