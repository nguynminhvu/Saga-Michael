using Microsoft.Extensions.Configuration;

namespace SagaPatternMichael.Product.Infrastructure.RabbitMQ.Events.Consumes
{
    public class InventoryCompletedCommand : MessageSupport
    {
        public InventoryCompletedCommand(IConfiguration configuration) : base(configuration)
        {
        }

        public override string Queue => "InventoryCompletedQueue";

        public override string Exchange => "InventoryCompletedExchange";

        public override string RoutingKey => "inventory-completed-routing-key";
    }
}
