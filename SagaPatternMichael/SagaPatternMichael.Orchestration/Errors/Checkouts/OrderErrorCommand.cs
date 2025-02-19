﻿using Microsoft.Extensions.Configuration;
using SagaPatternMichael.Orchestration.Helpers;
using SagaPatternMichael.Orchestration.Models;

namespace SagaPatternMichael.Orchestration.Errors.Checkouts
{
    public class OrderErrorCommand : MessageSupport
    {
        public OrderErrorCommand(IConfiguration configuration) : base(configuration)
        {
        }

        public override string Queue => "OrderErrorQueue";

        public override string Exchange => "OrderErrorExchange";

        public override string RoutingKey => "order-error-routing-key";
    }
}
