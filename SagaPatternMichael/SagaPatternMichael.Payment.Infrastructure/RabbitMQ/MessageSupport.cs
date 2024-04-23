﻿using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using SagaPatternMichael.Payment.DTOs;
using System.Text;

namespace SagaPatternMichael.Payment.Infrastructure.RabbitMQ
{
    public abstract class MessageSupport : MessageConnection
    {
        private readonly IConfiguration _configuration;
        public abstract string Queue { get; }
        public abstract string Exchange { get; }
        public abstract string RoutingKey { get; }

        public MessageSupport(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private void InitBroker(MessageChannel messageChannel)
        {
            if (_connection == null! || !_connection.IsOpen)
            {
                GetConnection(_configuration, messageChannel.Queue, messageChannel.Exchange, messageChannel.RoutingKey);
            }
            _channel.ExchangeDeclare(messageChannel.Exchange, ExchangeType.Direct);
            _channel.QueueDeclare(messageChannel.Queue, false, false, false, null!);
            _channel.QueueBind(messageChannel.Queue, messageChannel.Exchange, messageChannel.RoutingKey);
        }

        public async Task SendMessage(MessageDTO messageDTO, string queue, string exchange, string routingKey)
        {
            try
            {
                if (messageDTO == null!) throw new ArgumentNullException(nameof(messageDTO));

                InitBroker(new MessageChannel
                {
                    Exchange = exchange,
                    RoutingKey = routingKey,
                    Queue = queue
                });

                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageDTO));
                _channel.BasicPublish(exchange, routingKey, null!, body);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
