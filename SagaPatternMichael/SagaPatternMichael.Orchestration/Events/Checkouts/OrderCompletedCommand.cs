using Microsoft.Extensions.Configuration;
using SagaPatternMichael.Orchestration.Helpers;

namespace SagaPatternMichael.Orchestration.Events.Checkouts
{
    public class OrderCompletedCommand : MessageSupport
    {
        public OrderCompletedCommand(IConfiguration configuration) : base(configuration)
        {
        }
        public override string Queue => "OrderCompletedQueue";

        public override string Exchange => "OrderCompletedExchange";

        public override string RoutingKey => "order-completed-routing-key";
    }
}
