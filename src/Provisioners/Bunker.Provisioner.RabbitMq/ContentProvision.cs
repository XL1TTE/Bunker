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
            
            return rabbit;
        }
    }
}
