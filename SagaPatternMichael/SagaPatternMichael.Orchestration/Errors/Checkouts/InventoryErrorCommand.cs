using Microsoft.Extensions.Configuration;
using SagaPatternMichael.Orchestration.Helpers;

namespace SagaPatternMichael.Orchestration.Errors.Checkouts
{
    public class InventoryErrorCommand : MessageSupport
    {
        private readonly MessageSupport _messageSupport;

        public InventoryErrorCommand(IConfiguration configuration) : base(configuration)
        {
        }

        public override string Queue => "InventoryErrorQueue";

        public override string Exchange => "InventoryErrorExchange";

        public override string RoutingKey => "inventory-error-routing-key";
    }
}
