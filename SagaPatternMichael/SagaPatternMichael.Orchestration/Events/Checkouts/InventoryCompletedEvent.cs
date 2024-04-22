using Microsoft.Extensions.Configuration;
using SagaPatternMichael.Orchestration.Helpers;

namespace SagaPatternMichael.Orchestration.Events.Checkouts
{
    public class InventoryCompletedEvent : MessageSupport
    {
        public InventoryCompletedEvent(IConfiguration configuration) : base(configuration)
        {
        }
        public override string Queue => "PaymentQueue";

        public override string Exchange => "PaymentExchange";

        public override string RoutingKey => "payment-routing-key";
    }
}
