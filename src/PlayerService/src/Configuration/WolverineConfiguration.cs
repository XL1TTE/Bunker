using PlayerService.Persistence;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.RabbitMQ;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Wolverine.Postgresql;

namespace PlayerService.Configuration;

internal static class WolverineConfiguration
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigureWolverine()
        {
            builder.Services.AddWolverine(options =>
            {
                var mq = options.UseRabbitMqUsingNamedConnection("rabbit-mq");
                var db_connection = builder.Configuration.GetConnectionString("player-db");

                options.PersistMessagesWithPostgresql(db_connection ?? throw new Exception("Unable to find data base connection string!"));
                options.UseEntityFrameworkCoreTransactions();
            });

            return builder;
        }
    }
}
