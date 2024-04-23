using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SagaPatternMichael.Product.Infrastructure.Core.Entities;
using SagaPatternMichael.Product.Infrastructure.RabbitMQ.Events.Consumes;
using SagaPatternMichael.Product.Infrastructure.Services;
using System.Text;

namespace SagaPatternMichael.Product.Infrastructure.RabbitMQ.Consumes
{
    public class InventoryCompletedCommandConsume : BackgroundService, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly MessageConnection _messageConnection;
        private readonly IConfiguration _configuration;
        private readonly InventoryCompletedCommand _inventoryCompletedCommand;
        private readonly MessageEventFactory _messageFactory;
        private readonly EventFactory _test;
        private IModel _channel;

        public InventoryCompletedCommandConsume(MessageConnection messageConnection, IConfiguration configuration, IServiceScopeFactory serviceScopeFactory, MessageEventFactory messageEventFactory)
        {
            _scopeFactory = serviceScopeFactory;
            _messageConnection = messageConnection;
            _configuration = configuration;
            _inventoryCompletedCommand = new InventoryCompletedCommand(configuration);
            _messageFactory = messageEventFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _channel = _messageConnection.GetConnection(_configuration, _inventoryCompletedCommand.Queue, _inventoryCompletedCommand.Exchange, _inventoryCompletedCommand.RoutingKey);
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += async (s, e) =>
                {
                    var body = Encoding.UTF8.GetString(e.Body.ToArray());
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var productService = scope.ServiceProvider.GetService<IProductService>();
                        if (productService != null!)
                        {
                            await productService.AddEvent(new EventBox
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
                _channel.BasicConsume(_inventoryCompletedCommand.Queue, false, consumer);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at InventoryCompletedCommandConsume: " + ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
