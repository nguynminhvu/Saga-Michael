using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using SagaPatternMichael.Orchestration.Events.Checkouts;
using SagaPatternMichael.Orchestration.Helpers;
using SagaPatternMichael.Orchestration.Models;
using SagaPatternMichael.Orchestration.Services;

namespace SagaPatternMichael.Orchestration.OrchestrationConsume
{
    public class EventFactory : BackgroundService, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly MessageSupport _messageSupport;
        private CancellationTokenSource _cancellationTokenSource = new();
        public EventFactory(MessageSupport messageSupport, IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
        {
            _configuration = configuration;
            _scopeFactory = serviceScopeFactory;
            _messageSupport = messageSupport;
        }

        private async Task CommandHandler(MessageDTO messageDTO)
        {
            switch (messageDTO.Source)
            {
                // Do with specific business service, now is not need
                case "OrderCompletedEvent":
                    OrderCompletedEvent orderCompletedEvent = new(_configuration);
                    await orderCompletedEvent.SendMessage(messageDTO, orderCompletedEvent.Queue, orderCompletedEvent.Exchange, orderCompletedEvent.RoutingKey);
                    break;
                case "InventoryCompletedEvent":
                    InventoryCompletedEvent inventoryCompletedEvent = new InventoryCompletedEvent(_configuration);
                    await inventoryCompletedEvent.SendMessage(messageDTO, inventoryCompletedEvent.Queue, inventoryCompletedEvent.Exchange, inventoryCompletedEvent.RoutingKey);
                    break;
                case "PaymentCompletedEvent":
                    PaymentCompletedEvent paymentCompletedEvent = new PaymentCompletedEvent(_configuration);
                    await paymentCompletedEvent.SendMessage(messageDTO, paymentCompletedEvent.Queue, paymentCompletedEvent.Exchange, paymentCompletedEvent.RoutingKey);
                    break;
            }
        }

        public void StartOustandingEvent()
        {
            _cancellationTokenSource.Cancel();
        }

        private async Task PublishEvent(CancellationToken cancellationToken)
        {
            try
            {
                while (_cancellationTokenSource.IsCancellationRequested)
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var _orchestrationService = scope.ServiceProvider.GetService<IOrchestrationService>();
                        if (_orchestrationService == null!)
                        {
                            throw new InvalidOperationException();
                        }
                        var events = await _orchestrationService!.GetEvents();
                        foreach (var item in events)
                        {
                            MessageDTO messageDTO = JsonConvert.DeserializeObject<MessageDTO>(item.Data)!;
                            if (messageDTO != null!)
                            {
                                await CommandHandler(messageDTO);
                                await _orchestrationService.Remove(item);
                            }
                        }
                    }
                    using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);
                    try
                    {
                        await Task.Delay(Timeout.Infinite, linkedCts.Token);
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
            catch
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                await PublishEvent(stoppingToken);
            }
        }
    }
}
