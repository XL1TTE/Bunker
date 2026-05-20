using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.RabbitMQ;
using Wolverine.Postgresql;

namespace Bunker.AccountService.Api.Configuration;

internal static class WolverineConfiguration
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigureWolverine()
        {
            builder.Services.AddWolverine(options =>
            {
                var mq = options.UseRabbitMqUsingNamedConnection("rabbit-mq");
                var db_connection = builder.Configuration.GetConnectionString("account-db");

                options.PersistMessagesWithPostgresql(db_connection ?? throw new Exception("Unable to find data base connection string!"));
                options.UseEntityFrameworkCoreTransactions();
            });

            return builder;
        }
    }
}
