using Wolverine.RabbitMQ;
using Wolverine.RabbitMQ.Internal;

namespace Bunker.Provisioner.RabbitMq;

internal static class ContentProvision
{
    extension(RabbitMqTransportExpression rabbit)
    {
        internal RabbitMqTransportExpression ProvisionContent()
        {
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

            rabbit.DeclareExchange("card-pack-updates", e =>
            {
                e.ExchangeType = ExchangeType.Fanout;
            });

            rabbit.DeclareExchange("card-pack-deleted", e =>
            {
                e.ExchangeType = ExchangeType.Fanout;
            });

            rabbit.DeclareExchange("personality-preset-updates", e =>
            {
                e.ExchangeType = ExchangeType.Fanout;
            });

            rabbit.DeclareExchange("personality-preset-deleted", e =>
            {
                e.ExchangeType = ExchangeType.Fanout;
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

            return rabbit;
        }
    }
}