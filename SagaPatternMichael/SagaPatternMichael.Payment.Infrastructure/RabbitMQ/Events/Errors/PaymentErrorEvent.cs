using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaPatternMichael.Payment.Infrastructure.RabbitMQ.Events.Errors
{
    public class PaymentErrorEvent : MessageSupport
    {
        public PaymentErrorEvent(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
