using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace SagaPatternMichael.Payment.Infrastructure.RabbitMQ
{
    public abstract class MessageConnection
    {
        public IConnection _connection;
        public IModel _channel;

        public IModel GetConnection(IConfiguration configuration, string queue, string exchange, string routingKey)
        {
            try
            {
                ConnectionFactory factory = new ConnectionFactory();
                factory.HostName = configuration["RabbitMQHost"];
                factory.Port = Convert.ToInt32(configuration["RabbitMQPort"]);

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(exchange, ExchangeType.Direct);
                _channel.QueueDeclare(queue, false, false, false, null!);
                _channel.QueueBind(queue, exchange, routingKey);

                return _channel;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
