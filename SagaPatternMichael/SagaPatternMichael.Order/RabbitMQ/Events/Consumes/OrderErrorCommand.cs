
namespace SagaPatternMichael.Order.RabbitMQ.Events.Consumes
{
    public class OrderErrorCommand : MessageSupport
    {
        public OrderErrorCommand(IConfiguration configuration) : base(configuration)
        {
        }
        public override string Queue => "OrderErrorQueue";

        public override string Exchange => "OrderErrorExchange";

        public override string RoutingKey => "order-error-routing-key";
    }
}
