using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.RabbitMQ;
using Wolverine.Postgresql;
using Bunker.ContentService.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Bunker.ContentService.Messaging.Configuration;

internal static class WolverineConfiguration
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigureWolverine()
        {
            builder.Services.AddWolverine(options =>
            {
                var mq = options.UseRabbitMqUsingNamedConnection("rabbit-mq");
                var db_connection = builder.Configuration.GetConnectionString("content-service-db");

                options.CodeGeneration.AlwaysUseServiceLocationFor<ContentDbContext>();
                options.CodeGeneration.AlwaysUseServiceLocationFor<DbContextOptions<ContentDbContext>>();

                options.PersistMessagesWithPostgresql(db_connection ?? throw new Exception("Unable to find data base connection string!"));
                options.UseEntityFrameworkCoreTransactions();
            });

            return builder;
        }
    }
}
