using Wolverine.RabbitMQ;
using Wolverine.RabbitMQ.Internal;

namespace Bunker.Provisioner.RabbitMq;

internal static class ProvisionExtensions
{
    extension(RabbitMqTransportExpression rabbit)
    {
        internal RabbitMqTransportExpression ProvisionContent()
        {
            rabbit.DeclareExchange("account-updates", e =>
            {
                e.ExchangeType = ExchangeType.Fanout;

                e.BindQueue("lobby-service-account-updates");
                e.BindQueue("read-service-account-updates");
            });

            rabbit.DeclareExchange("sex-card-updates", e =>
            {
                e.ExchangeType = ExchangeType.Fanout;
                e.BindQueue("lobby-sex-card-updates-queue");
            });

            rabbit.DeclareExchange("profession-card-updates", e =>
            {
                e.ExchangeType = ExchangeType.Fanout;
                e.BindQueue("lobby-profession-card-updates-queue");
            });

            rabbit.DeclareExchange("fact-card-updates", e =>
            {
                e.ExchangeType = ExchangeType.Fanout;
                e.BindQueue("lobby-fact-card-updates-queue");
            });

            rabbit.DeclareExchange("age-card-updates", e =>
            {
                e.ExchangeType = ExchangeType.Fanout;
                e.BindQueue("lobby-age-card-updates-queue");
            });

            rabbit.DeclareExchange("hobbies-card-updates", e =>
            {
                e.ExchangeType = ExchangeType.Fanout;
                e.BindQueue("lobby-hobbies-card-updates-queue");
            });

            rabbit.DeclareExchange("card-deleted", e =>
            {
                e.ExchangeType = ExchangeType.Fanout;
            });

            rabbit.DeclareExchange("health-card-updates", e =>
            {
                e.ExchangeType = ExchangeType.Fanout;
            });

            rabbit.DeclareExchange("luggage-card-updates", e =>
            {
                e.ExchangeType = ExchangeType.Fanout;
            });

            rabbit.DeclareExchange("bunker-card-updates", e =>
            {
                e.ExchangeType = ExchangeType.Fanout;
            });

            rabbit.DeclareExchange("bunker-card-deleted", e =>
            {
                e.ExchangeType = ExchangeType.Fanout;
            });

            rabbit.DeclareExchange("game-start-requests", e =>
            {
                e.ExchangeType = ExchangeType.Fanout;
                e.BindQueue("game-service-game-start-requests");
            });

            rabbit.DeclareExchange("content-hydration-requests", e =>
            {
                e.ExchangeType = ExchangeType.Fanout;
                e.BindQueue("content-service-hydration-requests");
            });

            rabbit.DeclareExchange("game-content-hydrated", e =>
            {
                e.ExchangeType = ExchangeType.Fanout;
                e.BindQueue("game-service-game-content-hydrated");
            });

            rabbit.DeclareExchange("game-content-hydration-failed", e =>
            {
                e.ExchangeType = ExchangeType.Fanout;
                e.BindQueue("game-service-content-hydration-failed");
            });

            rabbit.DeclareExchange("game-start-succeeded", e =>
            {
                e.ExchangeType = ExchangeType.Fanout;
                e.BindQueue("lobby-service-game-start-succeeded");
            });

            rabbit.DeclareExchange("game-start-failed", e =>
            {
                e.ExchangeType = ExchangeType.Fanout;
                e.BindQueue("lobby-service-game-start-failed");
            });

            return rabbit;
        }
    }
}
