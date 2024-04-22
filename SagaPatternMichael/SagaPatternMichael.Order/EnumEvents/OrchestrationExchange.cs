namespace SagaPatternMichael.Order
{
    public static class OrchestrationExchange
    {
        public const string OrchestrationEvent = "OrchestrationExchangeEvent";
        public const string OrchestrationErrorEvent = "OrchestrationExchangeErrorEvent";
        public const string OrchestrationCommand = "OrchestrationExchangeCommand";
    }
    public static class OrchestrationQueue
    {
        public const string OrchestrationEvent = "OrchestrationQueueEvent";
        public const string OrchestrationErrorEvent = "OrchestrationQueueErrorEvent";
        public const string OrchestrationCommand = "OrchestrationQueueCommand";
    }
    public static class OrchestrationRoutingKey
    {
        public const string OrchestrationEvent = "orchestration-routing-key-event";
        public const string OrchestrationErrorEvent = "orchestration-routing-key-error-event";
        public const string OrchestrationCommand = "orchestration-routing-key-command";
    }
}
