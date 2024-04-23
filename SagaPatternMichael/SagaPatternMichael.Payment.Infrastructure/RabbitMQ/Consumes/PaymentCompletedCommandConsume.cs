using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SagaPatternMichael.Payment.Infrastructure.Core.Entities;
using SagaPatternMichael.Payment.Infrastructure.RabbitMQ.Events.Consumes;
using SagaPatternMichael.Payment.Infrastructure.Services;
using System.Text;

namespace SagaPatternMichael.Payment.Infrastructure.RabbitMQ.Consumes
{
    public class PaymentCompletedCommandConsume : BackgroundService, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly MessageConnection _messageConnection;
        private readonly IConfiguration _configuration;
        private readonly PaymentCompletedCommand _paymentCompletedCommand;
        private readonly MessageEventFactory _messageFactory;
        private IModel _channel;

        public PaymentCompletedCommandConsume(MessageConnection messageConnection,IConfiguration configuration, IServiceScopeFactory serviceScopeFactory, MessageEventFactory messageEventFactory)
        {
            _scopeFactory = serviceScopeFactory;
            _configuration = configuration;
            _messageConnection = messageConnection;
            _paymentCompletedCommand = new PaymentCompletedCommand(configuration);
            _messageFactory = messageEventFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _channel = _messageConnection.GetConnection(_configuration, _paymentCompletedCommand.Queue, _paymentCompletedCommand.Exchange, _paymentCompletedCommand.RoutingKey);
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += async (s, e) =>
                {
                    var body = Encoding.UTF8.GetString(e.Body.ToArray());
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var paymentService = scope.ServiceProvider.GetService<IPaymentService>();
                        if (paymentService != null!)
                        {
                            await paymentService.AddEvent(new EventBox
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
                _channel.BasicConsume(_paymentCompletedCommand.Queue, false, consumer);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at PaymentCompletedCommandConsume: " + ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
