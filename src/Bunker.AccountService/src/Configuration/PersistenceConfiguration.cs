using Bunker.AccountService.Persistence;
using Bunker.AccountService.Persistence.Repository;
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

            builder.Services.AddScoped<IUnitOfWork, AccountDbContext>(
                provider => provider.GetRequiredService<AccountDbContext>());

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
