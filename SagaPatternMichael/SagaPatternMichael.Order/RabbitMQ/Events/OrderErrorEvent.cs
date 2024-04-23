﻿
namespace SagaPatternMichael.Order.RabbitMQ.Events
{
    public class OrderErrorEvent : MessageSupport
    {
        public OrderErrorEvent(IConfiguration configuration) : base(configuration)
        {
        }
        public override string Queue => "OrderErrorQueue";

        public override string Exchange => "OrderErrorExchange";

        public override string RoutingKey => "order-error-routing-key";
    }
}
