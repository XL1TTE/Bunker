using Bunker.LobbyService.Messages;
using Bunker.LobbyService.Persistence;
using Bunker.LobbyService.Persistence.Abstractions;
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

            options.PersistMessagesWithPostgresql(
                 connectionString: db_connection ?? throw new Exception("Unable to find data base connection string!"),
                 schemaName: "wolverine"
             );

            options.UseEntityFrameworkCoreTransactions()
                .WithDbContextAbstraction<IUnitOfWork, LobbyDbContext>();

            options.Policies.AutoApplyTransactions();
            options.Policies.UseDurableOutboxOnAllSendingEndpoints();

            options.PublishMessage<GameStartRequested>()
                .ToRabbitExchange("game-start-requests")
                .UseDurableOutbox();

            options.ListenToRabbitQueue("lobby-service-account-updates")
                .DefaultIncomingMessage<AccountUpdated>()
                .UseDurableInbox();

            options.ListenToRabbitQueue("lobby-service-game-start-succeeded")
                .DefaultIncomingMessage<GameStartSucceeded>()
                .UseDurableInbox();

            options.ListenToRabbitQueue("lobby-service-game-start-progress")
                .DefaultIncomingMessage<GameStartProgress>()
                .UseDurableInbox();

            options.ListenToRabbitQueue("lobby-service-game-start-failed")
                .DefaultIncomingMessage<GameStartFailed>()
                .UseDurableInbox();

            options.ListenToRabbitQueue("lobby-service-game-finished")
                .DefaultIncomingMessage<GameFinished>()
                .UseDurableInbox();
        });
        return builder;
    }
}
