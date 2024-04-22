
namespace SagaPatternMichael.Order.RabbitMQ.Commands
{
    public class InventoryErrorCommand : MessageSupport
    {
        public InventoryErrorCommand(IConfiguration configuration) : base(configuration)
        {
        }

        public override string Queue => "";

        public override string Exchange => "";

        public override string RoutingKey => "";
    }
}
