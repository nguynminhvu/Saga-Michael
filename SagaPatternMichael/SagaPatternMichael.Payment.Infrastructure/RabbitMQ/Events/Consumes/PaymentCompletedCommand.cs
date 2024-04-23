using Microsoft.Extensions.Configuration;

namespace SagaPatternMichael.Payment.Infrastructure.RabbitMQ.Events.Consumes
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
