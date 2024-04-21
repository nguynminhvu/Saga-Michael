﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using SagaPatternMichael.Orchestration.Events;
using SagaPatternMichael.Orchestration.Helpers;
using SagaPatternMichael.Orchestration.Models;
using SagaPatternMichael.Orchestration.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaPatternMichael.Orchestration.OrchestrationConsume
{
    public class EventErrorFactory : BackgroundService, IDisposable
    {
        private CancellationTokenSource _cancellationTokenSource = new();
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly MessageSupport _messageSupport;

        public EventErrorFactory(MessageSupport messageSupport, IServiceScopeFactory serviceScopeFactory)
        {
            _scopeFactory = serviceScopeFactory;
            _messageSupport = messageSupport;
        }
        public void StartOustandingEvent()
        {
            _cancellationTokenSource.Cancel();
        }

        private async Task CommandHandler(MessageDTO messageDTO)
        {
            switch (messageDTO.Source)
            {
                case "OrderCompletedEvent": 
                    OrderCompletedEvent orderCompletedEvent = new OrderCompletedEvent(_messageSupport); 
                    await orderCompletedEvent.SendEvent(messageDTO); break;
            }
        }

        private async Task PublishEvent(CancellationToken cancellationToken)
        {
            try
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
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
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                await PublishEvent(stoppingToken);
            }
        }
    }
}
