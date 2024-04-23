using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SagaPatternMichael.Orchestration.EnumEvents;
using SagaPatternMichael.Orchestration.Helpers;
using SagaPatternMichael.Orchestration.Models;
using SagaPatternMichael.Orchestration.RabbitMQ.Configurations;
using SagaPatternMichael.Orchestration.Services;
using System.Text;
using System.Threading.Channels;

namespace SagaPatternMichael.Orchestration.OrchestrationConsume
{
    public class ErrorEventHandlerSg : BackgroundService, IDisposable
    {
        private readonly EventErrorFactory _eventFactory;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private readonly MessageConnection _messageConnection;
        private IModel _channel;

        public ErrorEventHandlerSg( IConfiguration configuration, IServiceScopeFactory serviceScopeFactory, EventErrorFactory eventFactory)
        {
            _eventFactory = eventFactory;
            _scopeFactory = serviceScopeFactory;
            _configuration = configuration;
            _messageConnection = new MessageSupport(_configuration);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _channel = _messageConnection
                  .GetConnectionMsg(_configuration, OrchestrationQueue.OrchestrationErrorEvent, OrchestrationExchange.OrchestrationErrorEvent, OrchestrationRoutingKey.OrchestrationErrorEvent);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (sender, args) =>
            {
                var body = Encoding.UTF8.GetString(args.Body.ToArray());
                if (!string.IsNullOrEmpty(body))
                {
                    MessageDTO messageDTO = JsonConvert.DeserializeObject<MessageDTO>(body)!;
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var _orchestrationService = scope.ServiceProvider.GetService<IOrchestrationService>();
                        if (_orchestrationService != null)
                        {
                            await _orchestrationService.AddMsgError(Data.EventErrorBox.Create(JsonConvert.SerializeObject(messageDTO)));
                            _eventFactory.StartOustandingEvent();
                        }
                        _channel.BasicAck(args.DeliveryTag, false);
                    }
                }
            };
            _channel.BasicConsume(OrchestrationQueue.OrchestrationErrorEvent, false, consumer);
            await Task.CompletedTask;
        }
    }
}
