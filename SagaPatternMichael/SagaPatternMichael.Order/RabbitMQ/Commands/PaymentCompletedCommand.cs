
namespace SagaPatternMichael.Order.RabbitMQ.Commands
{
    public class PaymentCompletedCommand : MessageSupport
    {
        public PaymentCompletedCommand(IConfiguration configuration) : base(configuration)
        {
        }
        public override string Queue => "PaymentCompletedQueue";

        public override string Exchange => "PaymentCompletedExchange";

        public override string RoutingKey => "payment-completed-routing-key";
    }
}
