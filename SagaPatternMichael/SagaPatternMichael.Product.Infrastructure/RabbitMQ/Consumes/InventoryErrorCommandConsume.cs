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
    public class InventoryErrorCommandConsume:BackgroundService,IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly MessageConnection _messageConnection;
        private readonly IConfiguration _configuration;
        private readonly InventoryErrorCommand _inventoryErrorCommand;
        private readonly MessageEventFactory _messageFactory;
        private IModel _channel;

        public InventoryErrorCommandConsume(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory, MessageEventFactory messageEventFactory)
        {
            _scopeFactory = serviceScopeFactory;
            _configuration = configuration;
            _messageConnection = new MessageSupport(_configuration);
            _inventoryErrorCommand = new InventoryErrorCommand(configuration);
            _messageFactory = messageEventFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _channel = _messageConnection.GetConnection(_configuration, _inventoryErrorCommand.Queue, _inventoryErrorCommand.Exchange, _inventoryErrorCommand.RoutingKey);
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += async (s, e) =>
                {
                    var body = Encoding.UTF8.GetString(e.Body.ToArray());
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var productService = scope.ServiceProvider.GetService<IProductService>();
                        if (productService != null!)
                        {
                            await productService.AddEventError(new EventErrorBox
                            {
                                Id = Guid.NewGuid(),
                                CreatedOn = DateTime.Now,
                                Data = body,
                                ModifiedOn = DateTime.Now
                            });
                            _messageFactory.StartCancelEvent();
                            _channel.BasicAck(e.DeliveryTag, false);
                        }
                    }
                };
                _channel.BasicConsume(_inventoryErrorCommand.Queue, false, consumer);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at InventoryErrorCommandConsume: " + ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
