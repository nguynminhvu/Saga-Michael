
namespace SagaPatternMichael.Order.RabbitMQ.Events.Consumes
{
    public class PaymentCompletedEvent : MessageSupport
    {

        public PaymentCompletedEvent(IConfiguration configuration) : base(configuration)
        {
        }

        public override string Queue => "PaymentCompletedQueue";

        public override string Exchange => "PaymentCompletedExchange";

        public override string RoutingKey => "payment-completed-routing-key";
    }
}
