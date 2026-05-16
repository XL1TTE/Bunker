using GameStateService.Persistence;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Postgresql;
using Wolverine.RabbitMQ;

namespace GameStateService.Configuration;

internal static class WolverineConfiguration
{
    internal static IHostApplicationBuilder ConfigureWolverine(this IHostApplicationBuilder builder)
    {
        builder.Services.AddWolverine(options =>
        {
            var mq = options.UseRabbitMqUsingNamedConnection("rabbit-mq");
            var db_connection = builder.Configuration.GetConnectionString("game-state-db");

            // Transactional Outbox with EF Core
            options.PersistMessagesWithPostgresql(db_connection ?? throw new Exception("Unable to find data base connection string!"));
            options.UseEntityFrameworkCoreTransactions();
            
        });

        return builder;
    }
}
