using Microsoft.Extensions.Configuration;
using SagaPatternMichael.Orchestration.Helpers;

namespace SagaPatternMichael.Orchestration.Events.Checkouts
{
    public class PaymentCompletedEvent:MessageSupport
    {
        public PaymentCompletedEvent(IConfiguration configuration) : base(configuration)
        {
        }

        public override string Queue => "OrderPaymentedQueue";

        public override string Exchange => "OrderPaymentedExchange";

        public override string RoutingKey => "order-paymented-routing-key";
    }
 
}
