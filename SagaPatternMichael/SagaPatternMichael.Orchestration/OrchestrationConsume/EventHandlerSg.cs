using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SagaPatternMichael.Orchestration.Data;
using SagaPatternMichael.Orchestration.EnumEvents;
using SagaPatternMichael.Orchestration.Helpers;
using SagaPatternMichael.Orchestration.Models;
using SagaPatternMichael.Orchestration.RabbitMQ.Configurations;
using SagaPatternMichael.Orchestration.Services;
using System.Text;
using System.Threading.Channels;

namespace SagaPatternMichael.Orchestration.OrchestrationConsume
{
    public class EventHandlerSg : BackgroundService, IDisposable
    {
        private readonly EventFactory _eventFactory;
        private readonly IServiceScopeFactory _serviceScope;
        private readonly IConfiguration _configuration;
        private readonly MessageConnection _messageConnection;
        private IModel _channel;

        public EventHandlerSg( IConfiguration configuration, IServiceScopeFactory serviceScopeFactory, EventFactory eventFactory)
        {
            _eventFactory = eventFactory;
            _serviceScope = serviceScopeFactory;
            _configuration = configuration;
            _messageConnection = new MessageSupport(_configuration);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _channel =
                _messageConnection.GetConnectionMsg(_configuration, OrchestrationQueue.OrchestrationEvent, OrchestrationExchange.OrchestrationEvent, OrchestrationRoutingKey.OrchestrationEvent);

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (sender, args) =>
            {
                var body = Encoding.UTF8.GetString(args.Body.ToArray());
                using (var scope = _serviceScope.CreateScope())
                {
                    var _orchestrationService = scope.ServiceProvider.GetService<IOrchestrationService>();
                    if (_orchestrationService != null)
                    {
                        var msgDTO = JsonConvert.DeserializeObject<MessageDTO>(body);
                        if (msgDTO != null!)
                        {
                            await _orchestrationService.AddMsg(EventBox.Create(JsonConvert.SerializeObject(msgDTO)));
                            _eventFactory.StartOustandingEvent();
                        }
                        _channel.BasicAck(args.DeliveryTag, false);
                    }
                }
            };
            _channel.BasicConsume(OrchestrationQueue.OrchestrationEvent, false, consumer);
            await Task.CompletedTask;
        }
    }
}
