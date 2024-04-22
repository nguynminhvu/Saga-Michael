using Microsoft.Extensions.Configuration;
using SagaPatternMichael.Orchestration.Helpers;

namespace SagaPatternMichael.Orchestration.Events.Checkouts
{
    public class InventoryCompletedCommand : MessageSupport
    {
        public InventoryCompletedCommand(IConfiguration configuration) : base(configuration)
        {
        }
        public override string Queue => "InventoryCompletedQueue";

        public override string Exchange => "InventoryCompletedExchange";

        public override string RoutingKey => "inventory-completed-routing-key";
    }
}
