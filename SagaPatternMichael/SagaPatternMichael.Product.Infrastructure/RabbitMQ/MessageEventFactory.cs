using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using SagaPatternMichael.Product.Infrastructure.DTOs;
using SagaPatternMichael.Product.Infrastructure.RabbitMQ.Events;
using SagaPatternMichael.Product.Infrastructure.Services;

namespace SagaPatternMichael.Product.Infrastructure.RabbitMQ
{
    public class MessageEventFactory : BackgroundService, IDisposable
    {
        private CancellationTokenSource _cancellationTokenSource = new();
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;

        public MessageEventFactory(IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
        {
            _configuration = configuration;
            _scopeFactory = serviceScopeFactory;
        }

        public void StartCancelEvent()
        {
            _cancellationTokenSource.Cancel();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await SendMessage(stoppingToken);
            }
        }

        private async Task SendMessage(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var _productService = scope.ServiceProvider.GetService<IProductService>();

                        if (_productService == null) throw new InvalidOperationException();

                        var events = await _productService.GetEvents();
                        foreach (var item in events)
                        {
                            var messageDTO = JsonConvert.DeserializeObject<MessageDTO>(item.Data);
                            if (messageDTO != null!)
                            {
                                await CommanHandler(messageDTO);
                                await _productService.RemoveEvent(item);
                            }
                        }
                        var eventErrors = await _productService.GetEventErrors();
                        foreach (var item in eventErrors)
                        {
                            var messageDTO = JsonConvert.DeserializeObject<MessageDTO>(item.Data);
                            if (messageDTO != null!)
                            {
                                await CommanHandler(messageDTO);
                                await _productService.RemoveEventError(item);
                            }
                        }
                    }
                    using var linkCts = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);
                    try
                    {
                        await Task.Delay(Timeout.Infinite, linkCts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        if (_cancellationTokenSource.Token.IsCancellationRequested)
                        {
                            var tmp = _cancellationTokenSource;
                            _cancellationTokenSource = new CancellationTokenSource();
                            tmp.Dispose();
                        }
                    }
                }
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at SendMessage" + ex.ToString());
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        private async Task CommanHandler(MessageDTO messageDTO)
        {
            try
            {
                switch (messageDTO.Source)
                {
                    case "InventoryCompletedCommand":
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var productService = scope.ServiceProvider.GetService<IProductService>();
                            if (productService != null)
                            {
                                var order = JsonConvert.DeserializeObject<OrderDTO>(messageDTO.Data);
                                if (order != null!)
                                {
                                    await productService.UpdateProduct(order);
                                    //await productService.UpdateProduct(null!);
                                }
                                InventoryCompletedEvent inventoryCompletedEvent = new InventoryCompletedEvent(_configuration);
                                messageDTO.Source = "InventoryCompletedEvent";
                                await inventoryCompletedEvent.SendMessage(messageDTO, OrchestrationQueue.OrchestrationEvent, OrchestrationExchange.OrchestrationEvent, OrchestrationRoutingKey.OrchestrationEvent);
                            }
                        }
                        break;

                    case "InventoryErrorCommand":
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var productService = scope.ServiceProvider.GetService<IProductService>();
                            if (productService != null)
                            {
                                var order = JsonConvert.DeserializeObject<OrderDTO>(messageDTO.Data);
                                if (order != null!)
                                {
                                    await productService.RollBackProduct(order);
                                }
                                InventoryErrorEvent inventoryErrorEvent = new InventoryErrorEvent(_configuration);
                                messageDTO.Source = "InventoryErrorEvent";
                                await inventoryErrorEvent.SendMessage(messageDTO, OrchestrationQueue.OrchestrationErrorEvent, OrchestrationExchange.OrchestrationErrorEvent, OrchestrationRoutingKey.OrchestrationErrorEvent);
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                InventoryErrorEvent inventoryErrorEvent = new InventoryErrorEvent(_configuration);
                messageDTO.Source = "InventoryErrorEvent";
                await inventoryErrorEvent.SendMessage(messageDTO, OrchestrationQueue.OrchestrationErrorEvent, OrchestrationExchange.OrchestrationErrorEvent, OrchestrationRoutingKey.OrchestrationErrorEvent);
            }
        }
    }
}
