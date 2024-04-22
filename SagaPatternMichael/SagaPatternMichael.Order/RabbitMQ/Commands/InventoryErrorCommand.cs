
namespace SagaPatternMichael.Order.RabbitMQ.Commands
{
    public class InventoryErrorCommand : MessageSupport
    {
        public InventoryErrorCommand(IConfiguration configuration) : base(configuration)
        {
        }

        public override string Queue => "InventoryErrorQueue";

        public override string Exchange => "InventoryErrorExchange";

        public override string RoutingKey => "inventory-error-routing-key";
    }
}
