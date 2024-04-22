using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaPatternMichael.Payment.Infrastructure.RabbitMQ.Events
{
    public class PaymentCompletedEvent : MessageSupport
    {
        public PaymentCompletedEvent(IConfiguration configuration) : base(configuration)
        {
        }

        // Default event orchestration
        public override string Queue => throw new NotImplementedException();

        public override string Exchange => throw new NotImplementedException();

        public override string RoutingKey => throw new NotImplementedException();
    }
}
