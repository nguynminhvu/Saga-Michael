using Microsoft.Extensions.Configuration;
using SagaPatternMichael.Orchestration.Helpers;
using SagaPatternMichael.Orchestration.Models;

namespace SagaPatternMichael.Orchestration.Errors.Checkouts
{
    public class OrderErrorEvent : MessageSupport
    {
        public OrderErrorEvent(IConfiguration configuration) : base(configuration)
        {
        }

        public override string Queue => "NotificationQueue";

        public override string Exchange => "NotificationExchange";

        public override string RoutingKey => "notification-routing-key";
    }
}
