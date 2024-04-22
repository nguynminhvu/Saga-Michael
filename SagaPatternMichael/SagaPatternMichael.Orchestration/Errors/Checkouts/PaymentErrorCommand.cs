using Microsoft.Extensions.Configuration;
using SagaPatternMichael.Orchestration.Helpers;

namespace SagaPatternMichael.Orchestration.Errors.Checkouts
{
    public class PaymentErrorCommand : MessageSupport
    {
        public PaymentErrorCommand(IConfiguration configuration) : base(configuration)
        {
        }

        public override string Queue => "PaymentErrorQueue";

        public override string Exchange => "PaymentErrorExchange";

        public override string RoutingKey => "payment-error-routing-key";
    }
}
