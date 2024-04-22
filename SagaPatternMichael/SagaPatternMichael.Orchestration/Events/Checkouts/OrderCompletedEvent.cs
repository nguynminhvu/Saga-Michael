using Microsoft.Extensions.Configuration;
using SagaPatternMichael.Orchestration.Helpers;

namespace SagaPatternMichael.Orchestration.Events.Checkouts
{
    public class OrderCompletedEvent : MessageSupport
    {
        public OrderCompletedEvent(IConfiguration configuration) : base(configuration)
        {
        }
        public override string Queue => "InventoryQueue";

        public override string Exchange => "InventoryExchange";

        public override string RoutingKey => "inventory-routing-key";
    }
}
