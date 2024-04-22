using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaPatternMichael.Product.Infrastructure.RabbitMQ.Events
{
    public class InventoryCompletedEvent : MessageSupport
    {
        public InventoryCompletedEvent(IConfiguration configuration) : base(configuration)
        {
        }

        // Default event orchestration
        public override string Queue => throw new NotImplementedException();

        public override string Exchange => throw new NotImplementedException();

        public override string RoutingKey => throw new NotImplementedException();
    }
}
