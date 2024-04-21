using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using SagaPatternMichael.Orchestration.Models;

namespace SagaPatternMichael.Orchestration.Helpers
{
    public class MessageSupport
    {
        private readonly IConfiguration _configuration;
        private IModel _channel;
        private IConnection _connection;

        public MessageSupport(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private void InitBus()
        {
            try
            {
                ConnectionFactory connectionFactory = new ConnectionFactory();
                connectionFactory.HostName = _configuration["RabbitMQHost"];
                connectionFactory.Port = Convert.ToInt32(_configuration["RabbitMQPort"]);
                _connection = connectionFactory.CreateConnection();
                _channel = _connection.CreateModel();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at MessageSupport.InitBus: " + ex.Message);
            }
        }

        private void InitBroker(MessageChannel messageChannel)
        {
            if (!_connection.IsOpen)
            {
                InitBus();
            }
            _channel.ExchangeDeclare(messageChannel.ExchangeName,)
        }
        public Task SendMessage(MessageDTO messageDTO)
        {
            try
            {
                if (messageDTO == null) throw new ArgumentNullException(nameof(messageDTO));
                var messageChannel = JsonConvert.DeserializeObject<MessageChannel>(messageDTO.Target);
                if (messageChannel == null!) throw new ArgumentNullException(nameof(messageChannel));

            }
            catch (Exception ex)
            {

            }
        }
    }
}
