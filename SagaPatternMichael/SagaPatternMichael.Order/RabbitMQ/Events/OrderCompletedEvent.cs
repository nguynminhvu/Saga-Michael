
namespace SagaPatternMichael.Order.RabbitMQ.Events
{
    public class OrderCompletedEvent : MessageSupport
    {
        public OrderCompletedEvent(IConfiguration configuration) : base(configuration)
        {
        }

        public override string Queue => throw new NotImplementedException();

        public override string Exchange => throw new NotImplementedException();

        public override string RoutingKey => throw new NotImplementedException();
    }
}
