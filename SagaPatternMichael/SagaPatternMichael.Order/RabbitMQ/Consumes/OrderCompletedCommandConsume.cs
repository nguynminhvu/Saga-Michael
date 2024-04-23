
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SagaPatternMichael.Order.Core.Entities;
using SagaPatternMichael.Order.RabbitMQ.Events.Consumes;
using SagaPatternMichael.Order.Services;
using System.Text;

namespace SagaPatternMichael.Order.RabbitMQ.Consumes
{
    public class OrderCompletedCommandConsume : BackgroundService, IDisposable
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IConfiguration _configuration;
        private readonly MessageConnection _messageConnection;
        private readonly MessageEventFactory _messageEventFactory;
        private readonly OrderCompletedCommand _orderCompletedCommand;
        private IModel _channel;

        public OrderCompletedCommandConsume(IConfiguration configuration, MessageConnection messageConnection, MessageEventFactory messageEventFactory, IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _configuration = configuration;
            _messageConnection = messageConnection;
            _messageEventFactory = messageEventFactory;
            _orderCompletedCommand = new OrderCompletedCommand(_configuration);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _channel = _messageConnection.GetConnection(_configuration, _orderCompletedCommand.Queue, _orderCompletedCommand.Exchange, _orderCompletedCommand.RoutingKey);
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += async (a, e) =>
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
                            _messageEventFactory.StartCancelEvent();
                        }
                        _channel.BasicAck(e.DeliveryTag, false);
                    }
                };
                _channel.BasicConsume(_orderCompletedCommand.Queue, false, consumer);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
