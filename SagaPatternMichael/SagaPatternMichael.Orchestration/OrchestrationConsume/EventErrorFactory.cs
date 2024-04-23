using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using SagaPatternMichael.Orchestration.Errors.Checkouts;
using SagaPatternMichael.Orchestration.Events;
using SagaPatternMichael.Orchestration.Models;
using SagaPatternMichael.Orchestration.Services;

namespace SagaPatternMichael.Orchestration.OrchestrationConsume
{
    public class EventErrorFactory : BackgroundService, IDisposable
    {
        private CancellationTokenSource _cancellationTokenSource = new();
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;

        public EventErrorFactory( IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
        {
            _configuration = configuration;
            _scopeFactory = serviceScopeFactory;
        }
        public void StartOustandingEvent()
        {
            _cancellationTokenSource.Cancel();
        }

        private async Task CommandHandler(MessageDTO messageDTO)
        {
            switch (messageDTO.Source)
            {
                case "OrderErrorEvent":
                    NotificationEvent notificationEvent = new NotificationEvent(_configuration);
                    messageDTO.Source = "NotificationEvent";
                    await notificationEvent.SendMessage(messageDTO, notificationEvent.Queue, notificationEvent.Exchange, notificationEvent.RoutingKey);
                    break;

                case "InventoryErrorEvent":
                    OrderErrorCommand orderErrorEvent = new OrderErrorCommand(_configuration);
                    messageDTO.Source = "OrderErrorCommand";
                    await orderErrorEvent.SendMessage(messageDTO, orderErrorEvent.Queue, orderErrorEvent.Exchange, orderErrorEvent.RoutingKey);
                    break;

                case "PaymentErrorEvent":
                    InventoryErrorCommand inventoryErrorEvent = new InventoryErrorCommand(_configuration);
                    messageDTO.Source = "InventoryErrorCommand";
                    await inventoryErrorEvent.SendMessage(messageDTO, inventoryErrorEvent.Queue, inventoryErrorEvent.Exchange, inventoryErrorEvent.RoutingKey);
                    break;
            }
        }

        private async Task PublishEvent(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var _orchestrationService = scope.ServiceProvider.GetService<IOrchestrationService>();
                        if (_orchestrationService != null)
                        {
                            var eventErrors = await _orchestrationService.GetEventsError();
                            foreach (var item in eventErrors)
                            {
                                var messageDTO = JsonConvert.DeserializeObject<MessageDTO>(item.Data);
                                if (messageDTO != null)
                                {
                                    await CommandHandler(messageDTO);
                                    await _orchestrationService.RemoveError(item);
                                }
                            }
                        }
                    }
                    using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);
                    try
                    {
                        await Task.Delay(Timeout.Infinite, cancellationToken);
                    }
                    catch
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
            while (!stoppingToken.IsCancellationRequested)
            {
                await PublishEvent(stoppingToken);
            }
        }
    }
}
