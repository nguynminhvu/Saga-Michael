using RabbitMQ.Client;

namespace SagaPatternMichael.Order.RabbitMQ
{
    public abstract class MessageConnection
    {
        protected IConnection _connection;
        protected IModel _channel;
        public abstract string Queue { get; }
        public abstract string Exchange { get; }
        public abstract string RoutingKey { get; }
        public IModel GetConnection(IConfiguration configuration, string queueName, string exchangeName, string routingKey)
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = configuration["RabbitMQHost"];
            factory.Port = Convert.ToInt32(configuration["RabbitMQPort"]);

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            _channel.QueueDeclare(queueName, false, false, false, null!);
            _channel.QueueBind(queueName, exchangeName, routingKey);

            return _channel;
        }

        protected abstract void InitBroker(MessageChannel messageChannel);
    }
}
