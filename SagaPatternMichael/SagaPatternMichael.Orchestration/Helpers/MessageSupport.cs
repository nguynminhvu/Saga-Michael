using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using SagaPatternMichael.Orchestration.Models;
using SagaPatternMichael.Orchestration.RabbitMQ.Configurations;
using System.Text;

namespace SagaPatternMichael.Orchestration.Helpers
{
    public class MessageSupport : MessageConnection
    {
        private readonly IConfiguration _configuration;

        public MessageSupport(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void InitBroker(MessageChannel messageChannel)
        {
            if (!_connection.IsOpen)
            {
                base.GetConnectionMsg(_configuration, messageChannel.QueueName, messageChannel.ExchangeName, messageChannel.RoutingKey);
            }
            _channel.ExchangeDeclare(messageChannel.ExchangeName, ExchangeType.Direct);
            _channel.QueueDeclare(messageChannel.QueueName, false, false, false, null!);
            _channel.QueueBind(messageChannel.QueueName, messageChannel.ExchangeName, messageChannel.RoutingKey);
        }

        public async Task SendMessage(MessageDTO messageDTO, string queue, string exchange, string routingKey)
        {
            try
            {
                if (messageDTO == null) throw new ArgumentNullException(nameof(messageDTO));
                InitBroker(new MessageChannel {ExchangeName=exchange,QueueName=queue,RoutingKey=routingKey });

                var rawSerial = JsonConvert.SerializeObject(messageDTO.Data);
                var body = Encoding.UTF8.GetBytes(rawSerial);
                _channel.ConfirmSelect();
                _channel.BasicPublish(exchange, routingKey, null!, body);
                if (!_channel.WaitForConfirms(new TimeSpan(0, 0, 5)))
                {
                    Console.WriteLine("Send fail");
                }
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new Exception("Error at SendMessage: " + ex.Message);
            }
        }
    }
}
