namespace SagaPatternMichael.Product.Infrastructure.RabbitMQ
{
    public class MessageChannel
    {
        public string Queue { get; set; }
        public string Exchange { get; set; }
        public string RoutingKey { get; set; }
    }
}
