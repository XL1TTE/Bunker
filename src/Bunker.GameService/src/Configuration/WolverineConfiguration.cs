using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.RabbitMQ;
using Wolverine.Postgresql;
using Bunker.GameService.Persistence;
using Bunker.GameService.Messages;
using Microsoft.EntityFrameworkCore;
using Bunker.GameService.Persistence.Contracts;

namespace Bunker.GameService.Messaging.Configuration;

internal static class WolverineConfiguration
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigureWolverine()
        {
            builder.Services.AddWolverine(options =>
            {
                options.UseRabbitMqUsingNamedConnection("rabbit-mq");
                var dbConnection = builder.Configuration.GetConnectionString("game-state-db");

                options.CodeGeneration.AlwaysUseServiceLocationFor<GameDbContext>();
                options.CodeGeneration.AlwaysUseServiceLocationFor<DbContextOptions<GameDbContext>>();
                options.CodeGeneration.AlwaysUseServiceLocationFor<IUnitOfWork>();

                options.PersistMessagesWithPostgresql(
                    connectionString: dbConnection ?? throw new Exception("Unable to find data base connection string!"),
                    schemaName: "wolverine"
                );

                options.UseEntityFrameworkCoreTransactions()
                    .WithDbContextAbstraction<IUnitOfWork, GameDbContext>();

                options.Policies.AutoApplyTransactions();

                options.ConfigureMessaging();
            });

            return builder;
        }
    }

    private static WolverineOptions ConfigureMessaging(this WolverineOptions options)
    {
        options.Policies.UseDurableOutboxOnAllSendingEndpoints();

        options.ListenToRabbitQueue("game-service-game-start-requests")
            .DefaultIncomingMessage<GameStartRequested>()
            .UseDurableInbox();

        options.ListenToRabbitQueue("game-service-game-content-hydrated")
            .DefaultIncomingMessage<GameContentHydrated>()
            .UseDurableInbox();

        options.ListenToRabbitQueue("game-service-content-hydration-failed")
            .DefaultIncomingMessage<GameContentHydrationFailed>()
            .UseDurableInbox();

        options.PublishMessage<RequestGameContentHydration>().ToRabbitExchange("content-hydration-requests");
        options.PublishMessage<GameStartProgress>().ToRabbitExchange("game-start-progress");
        options.PublishMessage<GameStartSucceeded>().ToRabbitExchange("game-start-succeeded");
        options.PublishMessage<GameStartFailed>().ToRabbitExchange("game-start-failed");
        options.PublishMessage<GameFinished>().ToRabbitExchange("game-finished");

        return options;
    }
}