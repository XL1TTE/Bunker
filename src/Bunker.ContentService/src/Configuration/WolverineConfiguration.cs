using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.RabbitMQ;
using Wolverine.Postgresql;
using Bunker.ContentService.Persistence;
using Microsoft.EntityFrameworkCore;
using Bunker.ContentService.Persistence.Contracts;

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
                options.CodeGeneration.AlwaysUseServiceLocationFor<IUnitOfWork>();

                options.PersistMessagesWithPostgresql(
                    connectionString: db_connection ?? throw new Exception("Unable to find data base connection string!"),
                    schemaName: "wolverine"
                );

                options.UseEntityFrameworkCoreTransactions()
                    .WithDbContextAbstraction<IUnitOfWork, ContentDbContext>();

                options.Policies.AutoApplyTransactions();

                options.ConfigureMessaging();
            });

            return builder;
        }
    }

    private static WolverineOptions ConfigureMessaging(this WolverineOptions options)
    {
        options.Policies.UseDurableOutboxOnAllSendingEndpoints();

        options.PublishMessage<Messages.SexCardUpdated>().ToRabbitExchange("sex-card-updates");
        options.PublishMessage<Messages.ProfessionCardUpdated>().ToRabbitExchange("profession-card-updates");
        options.PublishMessage<Messages.FactCardUpdated>().ToRabbitExchange("fact-card-updates");
        options.PublishMessage<Messages.AgeCardUpdated>().ToRabbitExchange("age-card-updates");
        options.PublishMessage<Messages.HobbiesCardUpdated>().ToRabbitExchange("hobbies-card-updates");

        return options;
    }
}
