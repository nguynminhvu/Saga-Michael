
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SagaPatternMichael.Order.Core.Entities;
using SagaPatternMichael.Order.RabbitMQ.Events.Consumes;
using SagaPatternMichael.Order.Services;
using System.Text;

namespace SagaPatternMichael.Order.RabbitMQ.Consumes
{
    public class PaymentCompletedEventConsume : BackgroundService, IDisposable
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IConfiguration _configuration;
        private readonly MessageConnection _messageConnection;
        private readonly MessageEventFactory _messageEventFactory;
        private readonly PaymentCompletedEvent _paymentCompletedEvent;
        private IModel _channel;

        public PaymentCompletedEventConsume(IConfiguration configuration, MessageConnection messageConnection, MessageEventFactory messageEventFactory,IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory= serviceScopeFactory;
            _configuration = configuration;
            _messageConnection = messageConnection;
            _messageEventFactory = messageEventFactory;
            _paymentCompletedEvent = new PaymentCompletedEvent(_configuration);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _channel = _messageConnection.GetConnection(_configuration, _paymentCompletedEvent.Queue, _paymentCompletedEvent.Exchange, _paymentCompletedEvent.RoutingKey);
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (a, e) =>
            {
                var body = Encoding.UTF8.GetString(e.Body.ToArray());
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var orderService = scope.ServiceProvider.GetService<IOrderService>();
                    if (orderService != null!)
                    {
                        await orderService.AddEvent(new EventBox
                        {
                            Id = Guid.NewGuid(),
                            CreatedOn = DateTime.Now,
                            Data = body,
                            ModifiedOn = DateTime.Now
                        });
                        _messageFactory.StartCancelEvent();
                    }
                    _channel.BasicAck(e.DeliveryTag, false);
                }
            };
        }
    }
}
