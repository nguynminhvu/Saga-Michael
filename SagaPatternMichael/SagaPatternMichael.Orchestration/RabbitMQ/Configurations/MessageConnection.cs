using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using SagaPatternMichael.Orchestration.Models;

namespace SagaPatternMichael.Orchestration.RabbitMQ.Configurations
{
    public abstract class MessageConnection
    {
        protected IConnection _connection;
        protected IModel _channel;
        public IModel GetConnectionMsg(IConfiguration configuration, string queueName, string exchangeName, string routingKey)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null!;
            }
        }
        protected abstract void InitBroker(MessageChannel messageChannel);
    }
}
