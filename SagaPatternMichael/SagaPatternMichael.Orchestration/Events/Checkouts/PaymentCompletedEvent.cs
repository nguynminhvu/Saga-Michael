using Microsoft.Extensions.Configuration;
using SagaPatternMichael.Orchestration.Helpers;

namespace SagaPatternMichael.Orchestration.Events.Checkouts
{
    public class PaymentCompletedEvent:MessageSupport
    {
        public PaymentCompletedEvent(IConfiguration configuration) : base(configuration)
        {
        }

        public override string Queue => "NotificationQueue";

        public override string Exchange => "NotificationExchange";

        public override string RoutingKey => "notification-routing-key";
    }
}
