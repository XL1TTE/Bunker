using PlayerService.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PlayerService.Configuration;

internal static class PersistenceConfiguration
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder IncludePersistence()
        {
            var connectionString = builder.Configuration.GetConnectionString("player-db");
            builder.Services.AddDbContext<PlayerDbContext>(options =>
                options.UseNpgsql(connectionString));
            return builder;
        }
    }

    extension(WebApplication app)
    {
        public async Task InitializeDatabaseAsync()
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PlayerDbContext>();
            await context.Database.EnsureCreatedAsync();
        }
    }
}
