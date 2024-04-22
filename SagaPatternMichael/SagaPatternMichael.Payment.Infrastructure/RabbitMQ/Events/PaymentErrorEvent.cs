using Microsoft.Extensions.Configuration;
using SagaPatternMichael.Payment.Infrastructure.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaPatternMichael.Product.Infrastructure.RabbitMQ.Events
{
    public class PaymentErrorEvent : MessageSupport
    {
        public PaymentErrorEvent(IConfiguration configuration) : base(configuration)
        {
        }

        public override string Queue => throw new NotImplementedException();

        public override string Exchange => throw new NotImplementedException();

        public override string RoutingKey => throw new NotImplementedException();
    }
}
