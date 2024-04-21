using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace SagaPatternMichael.Orchestration.RabbitMQ.Configurations
{
    public static class MessageConnection
    {
        private static IConnection _connection;
        private static IModel _channel;

        public static IModel GetConnectionMsg(this IConfiguration configuration, string queueName, string exchangeName, string routingKey)
        {
            ConnectionFactory connectionFactory = new ConnectionFactory();
            connectionFactory.HostName = configuration["RabbitMQHost"];
            connectionFactory.Port = Convert.ToInt32(configuration["RabbitMQPort"]);
            connectionFactory.RequestedHeartbeat = TimeSpan.FromSeconds(60);

            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            _channel.QueueDeclare(queueName, false, false, false, null!);
            _channel.QueueBind(queueName, exchangeName, routingKey);
            return _channel;
        }
    }
}
