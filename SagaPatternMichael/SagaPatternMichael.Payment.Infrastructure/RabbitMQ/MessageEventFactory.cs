
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using SagaPatternMichael.Payment.DTOs;
using SagaPatternMichael.Payment.Infrastructure.DTOs;
using SagaPatternMichael.Payment.Infrastructure.RabbitMQ.Events;
using SagaPatternMichael.Payment.Infrastructure.RabbitMQ.Events.Errors;
using SagaPatternMichael.Payment.Infrastructure.Services;

namespace SagaPatternMichael.Payment.Infrastructure.RabbitMQ
{
    public class MessageEventFactory : BackgroundService, IDisposable
    {
        CancellationTokenSource _cancellationTokenSource = new();
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
                        var _paymentService = scope.ServiceProvider.GetService<IPaymentService>();

                        if (_paymentService == null) throw new InvalidOperationException();

                        var events = await _paymentService.GetEvents();
                        foreach (var item in events)
                        {
                            var messageDTO = JsonConvert.DeserializeObject<MessageDTO>(item.Data);
                            if (messageDTO != null!)
                            {
                                await CommanHandler(messageDTO);
                                await _paymentService.RemoveEvent(item);
                            }
                        }
                    }
                    using var linkCts = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);
                    try
                    {
                        await Task.Delay(Timeout.Infinite, linkCts.Token);
                    }
                    catch(OperationCanceledException)
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
                    case "PaymentCompletedCommand":
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var paymentService = scope.ServiceProvider.GetService<IPaymentService>();
                            if (paymentService != null)
                            {
                                var order = JsonConvert.DeserializeObject<OrderDTO>(messageDTO.Data);
                                if (order != null!)
                                {
                                    await paymentService.Payment(order);
                                    //await paymentService.Payment(null!);
                                }
                                PaymentCompletedEvent paymentCompletedEvent = new PaymentCompletedEvent(_configuration);
                                messageDTO.Source = "PaymentCompletedEvent";
                                await paymentCompletedEvent.SendMessage(messageDTO, OrchestrationQueue.OrchestrationEvent, OrchestrationExchange.OrchestrationEvent, OrchestrationRoutingKey.OrchestrationEvent);
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                PaymentErrorEvent paymentErrorEvent = new PaymentErrorEvent(_configuration);
                messageDTO.Source = "PaymentErrorEvent";
                await paymentErrorEvent.SendMessage(messageDTO, OrchestrationQueue.OrchestrationErrorEvent, OrchestrationExchange.OrchestrationErrorEvent, OrchestrationRoutingKey.OrchestrationErrorEvent);

            }
        }
    }
}
