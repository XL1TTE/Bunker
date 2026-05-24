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
            builder.Services.AddDbContext<ContentDbContext>(options =>
                options.UseNpgsql(connectionString));

            builder.Services.AddScoped<IUnitOfWork, ContentDbContext>();
            builder.Services.AddScoped<ICardQueries, DbContextCardQueries>();

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
