
namespace SagaPatternMichael.Order.RabbitMQ.Events
{
    public class OrderErrorEvent : MessageSupport
    {
        public OrderErrorEvent(IConfiguration configuration) : base(configuration)
        {
        }

        public override string Queue => throw new NotImplementedException();

        public override string Exchange => throw new NotImplementedException();

        public override string RoutingKey => throw new NotImplementedException();
    }
}
