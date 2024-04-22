using Microsoft.Extensions.Configuration;
using SagaPatternMichael.Orchestration.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaPatternMichael.Orchestration.Events
{
    public class NotificationEvent : MessageSupport
    {
        public NotificationEvent(IConfiguration configuration) : base(configuration)
        {
        }

        public override string Queue => throw new NotImplementedException();

        public override string Exchange => throw new NotImplementedException();

        public override string RoutingKey => throw new NotImplementedException();
    }
}
