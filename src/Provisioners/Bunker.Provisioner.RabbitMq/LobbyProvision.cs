using Wolverine.RabbitMQ;
using Wolverine.RabbitMQ.Internal;

namespace Bunker.Provisioner.RabbitMq;

internal static class LobbyProvision
{
    extension(RabbitMqTransportExpression rabbit)
    {
        internal RabbitMqTransportExpression ProvisionLobby()
        {
            rabbit.DeclareExchange("game-start-requests", e =>
            {
                e.ExchangeType = ExchangeType.Fanout;
                e.BindQueue("game-service-game-start-requests");
            });

            return rabbit;
        }
    }
}