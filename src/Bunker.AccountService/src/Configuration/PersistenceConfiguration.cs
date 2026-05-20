using Bunker.AccountService.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Bunker.AccountService.Api.Configuration;

internal static class PersistenceConfiguration
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder IncludePersistence()
        {
            var connectionString = builder.Configuration.GetConnectionString("account-db");
            builder.Services.AddDbContext<AccountDbContext>(options =>
                options.UseNpgsql(connectionString));
            return builder;
        }
    }

    extension(WebApplication app)
    {
        public async Task InitializeDatabaseAsync()
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
            await context.Database.EnsureCreatedAsync();
        }
    }
}
