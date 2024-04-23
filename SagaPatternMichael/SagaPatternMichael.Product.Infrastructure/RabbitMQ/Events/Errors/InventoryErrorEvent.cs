using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaPatternMichael.Product.Infrastructure.RabbitMQ.Events.Errors
{
    public class InventoryErrorEvent : MessageSupport
    {
        public InventoryErrorEvent(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
