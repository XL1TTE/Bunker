using Wolverine.RabbitMQ;
using Wolverine.RabbitMQ.Internal;

namespace Bunker.Provisioner.RabbitMq;

internal static class AccountProvision
{
    extension(RabbitMqTransportExpression rabbit)
    {
        internal RabbitMqTransportExpression ProvisionAccount()
        {
            rabbit.DeclareExchange("account-updates", e =>
            {
                e.ExchangeType = ExchangeType.Fanout;

                e.BindQueue("lobby-service-account-updates");
                e.BindQueue("read-service-account-updates");
            });

            return rabbit;
        }
    }
}