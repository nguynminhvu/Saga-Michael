using Newtonsoft.Json;
using RabbitMQ.Client;
using SagaPatternMichael.Order.DTOs;
using System.Text;

namespace SagaPatternMichael.Order.RabbitMQ
{
    public  class MessageSupport : MessageConnection
    {
        private readonly IConfiguration _configuration;

        public MessageSupport(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override string Queue => throw new NotImplementedException();

        public override string Exchange => throw new NotImplementedException();

        public override string RoutingKey => throw new NotImplementedException();

        public async Task SendMessage(MessageDTO messageDTO, string queue, string exchange, string routingKey)
        {
            try
            {
                if (messageDTO == null) throw new ArgumentNullException(nameof(messageDTO));

                InitBroker(new MessageChannel
                {
                    Exchange = exchange,
                    RoutingKey = routingKey,
                    Queue = queue
                });

                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageDTO));
                _channel.ConfirmSelect();
                _channel.BasicPublish(exchange, routingKey, null!, body);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at SendMessage: " + ex.ToString());
            }
        }

        protected override void InitBroker(MessageChannel messageChannel)
        {
            if (_connection == null! || !_connection.IsOpen)
            {
                GetConnection(_configuration, messageChannel.Queue, messageChannel.Exchange, messageChannel.RoutingKey);
            }
            _channel.ExchangeDeclare(messageChannel.Exchange, ExchangeType.Direct);
            _channel.QueueDeclare(messageChannel.Queue, false, false, false, null!);
            _channel.QueueBind(messageChannel.Queue, messageChannel.Exchange, messageChannel.RoutingKey);
        }
    }
}
