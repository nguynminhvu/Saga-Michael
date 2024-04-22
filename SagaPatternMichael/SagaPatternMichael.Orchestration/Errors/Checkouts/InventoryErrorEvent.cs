using Microsoft.Extensions.Configuration;
using SagaPatternMichael.Orchestration.Helpers;

namespace SagaPatternMichael.Orchestration.Errors.Checkouts
{
    public class InventoryErrorEvent : MessageSupport
    {
        private readonly MessageSupport _messageSupport;

        public InventoryErrorEvent(IConfiguration configuration) : base(configuration)
        {
        }

        public override string Queue => "OrderRollQueue";

        public override string Exchange => "OrderRollExchange";

        public override string RoutingKey => "order-roll-routing-key";
    }
}
