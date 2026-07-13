using Wolverine.RabbitMQ;
using Wolverine.RabbitMQ.Internal;

namespace Bunker.Provisioner.RabbitMq;

internal static class GameProvision
{
    extension(RabbitMqTransportExpression rabbit)
    {
        internal RabbitMqTransportExpression ProvisionGame()
        {
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

            rabbit.DeclareExchange("game-start-progress", e =>
            {
                e.ExchangeType = ExchangeType.Fanout;
                e.BindQueue("lobby-service-game-start-progress");
            });

            rabbit.DeclareExchange("game-finished", e =>
            {
                e.ExchangeType = ExchangeType.Fanout;
                e.BindQueue("lobby-service-game-finished");
            });

            rabbit.DeclareExchange("content-hydration-requests", e =>
            {
                e.ExchangeType = ExchangeType.Fanout;
                e.BindQueue("content-service-hydration-requests");
            });

            return rabbit;
        }
    }
}
