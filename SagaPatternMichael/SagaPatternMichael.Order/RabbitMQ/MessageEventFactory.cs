
using Newtonsoft.Json;
using SagaPatternMichael.Order.DTOs;
using SagaPatternMichael.Order.RabbitMQ.Commands;
using SagaPatternMichael.Order.RabbitMQ.Events;
using SagaPatternMichael.Order.Services;

namespace SagaPatternMichael.Order.RabbitMQ
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
                        var _orderService = scope.ServiceProvider.GetService<IOrderService>();

                        if (_orderService == null) throw new InvalidOperationException();

                        var events = await _orderService.GetEvents();
                        foreach (var item in events)
                        {
                            var messageDTO = JsonConvert.DeserializeObject<MessageDTO>(item.Data);
                            if (messageDTO != null!)
                            {
                                await CommanHandler(messageDTO);
                                await _orderService.RemoveEvent(item);
                            }
                        }
                    }
                    using var linkCts = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);
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
            catch (Exception ex)
            {
                Console.WriteLine("Error at SendMessage" + ex.ToString());
                await Task.Delay(TimeSpan.FromSeconds(1));

            }
        }
        private async Task CommanHandler(MessageDTO messageDTO)
        {
            switch (messageDTO.Source)
            {
                case "InventoryErrorCommand":
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var orderService = scope.ServiceProvider.GetService<IOrderService>();
                        if (orderService != null)
                        {
                            InventoryErrorCommand orderPaymentedCommand = new InventoryErrorCommand(_configuration);
                            var order = JsonConvert.DeserializeObject<Core.Entities.Order>(messageDTO.Data);
                            if (order != null!)
                            {
                                order.Update("Payment");
                                await orderService.UpdateOrder(order);
                            }
                            await orderPaymentedCommand.SendMessage(messageDTO, orderPaymentedCommand.Queue, orderPaymentedCommand.Exchange, orderPaymentedCommand.RoutingKey);
                        }
                    }
                    break;
                case "PaymentCompletedCommand":
                    NotificationEvent notificationEvent = new NotificationEvent(_configuration);
                    await notificationEvent.SendMessage(messageDTO, notificationEvent.Queue, notificationEvent.Exchange, notificationEvent.RoutingKey);
                    break;
            }
        }
    }
}
