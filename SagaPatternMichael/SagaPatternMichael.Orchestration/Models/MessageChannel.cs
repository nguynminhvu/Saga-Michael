namespace SagaPatternMichael.Orchestration.Models
{
    public class MessageChannel
    {
        public string QueueName { get; set; }
        public string ExchangeName { get; set; }
        public string RoutingKey { get; set; }
    }
}
