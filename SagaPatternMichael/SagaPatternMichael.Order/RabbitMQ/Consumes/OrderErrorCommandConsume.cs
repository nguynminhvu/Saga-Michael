
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SagaPatternMichael.Order.RabbitMQ.Events;
using SagaPatternMichael.Order.Services;
using System.Text;

namespace SagaPatternMichael.Order.RabbitMQ.Consumes
{
    public class OrderErrorCommandConsume : BackgroundService, IDisposable
    {
        private readonly MessageEventFactory _messageEventFactory;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private readonly MessageConnection _messageConnection;
        private readonly OrderErrorEvent _orderErrorEvent;
        private IModel _channel;

        public OrderErrorCommandConsume(MessageConnection messageConnection, IConfiguration configuration, IServiceScopeFactory serviceScopeFactory, MessageEventFactory messageEventFactory)
        {
            _messageEventFactory = messageEventFactory;
            _scopeFactory = serviceScopeFactory;
            _configuration = configuration;
            _messageConnection = messageConnection;
            _orderErrorEvent = new OrderErrorEvent(_configuration);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _channel = _messageConnection.GetConnection(_configuration, _orderErrorEvent.Queue, _orderErrorEvent.Exchange, _orderErrorEvent.RoutingKey);

                var consumer = new EventingBasicConsumer(_channel);

                consumer.Received += async (sender, args) =>
                {
                    var body = Encoding.UTF8.GetString(args.Body.ToArray());

                    if (!string.IsNullOrEmpty(body))
                    {
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var orderService = scope.ServiceProvider.GetService<IOrderService>();
                            if (orderService != null!)
                            {
                                await orderService.AddEvent(new Core.Entities.EventBox
                                {
                                    Id = Guid.NewGuid(),
                                    CreatedOn = DateTime.Now,
                                    ModifiedOn = DateTime.Now,
                                    Data = body
                                });
                                _messageEventFactory.StartCancelEvent();
                            }
                            _channel.BasicAck(args.DeliveryTag, false);
                        }
                    }
                };
                _channel.BasicConsume(_orderErrorEvent.Queue, false, consumer);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
