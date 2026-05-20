using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Postgresql;
using Wolverine.RabbitMQ;

namespace Bunker.LobbyService.Messaging.Configuration;

internal static class WolverineConfiguration
{
    internal static IHostApplicationBuilder ConfigureWolverine(this IHostApplicationBuilder builder)
    {
        builder.Services.AddWolverine(options =>
        {
            var db_connection = builder.Configuration.GetConnectionString("lobby-db");

            options.UseRabbitMqUsingNamedConnection("rabbit-mq");
            options.PersistMessagesWithPostgresql(db_connection ?? throw new Exception("Unable to find database connection string!"));
            options.UseEntityFrameworkCoreTransactions();
        });
        return builder;
    }
}
