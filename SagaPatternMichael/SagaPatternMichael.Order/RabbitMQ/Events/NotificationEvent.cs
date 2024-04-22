
namespace SagaPatternMichael.Order.RabbitMQ.Events
{
    public class NotificationEvent : MessageSupport
    {
        public NotificationEvent(IConfiguration configuration) : base(configuration)
        {
        }

        public override string Queue => "NotificationQueue";

        public override string Exchange => "NotificationExchange";

        public override string RoutingKey => "notification-routing-key";
    }
}
