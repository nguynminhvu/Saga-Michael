namespace SagaPatternMichael.Order.EnumEvents
{
    public class OrderExchange
    {
        // Listen all event, command came to the event queue (Order), include Payment Service proccess successed raise event 
        public const string OrderEventExchange = "OrderEventExchange";
        public const string OrderCommandExchange = "OrderCommandExchange";
    }
    public class OrderQueue
    {
        public const string OrderEventQueue = "OrderEventQueue";
        public const string OrderCommandQueue = "OrderCommandQueue";
    }
    public class OrderRouting
    {
        public const string OrderEventRouting = "order-event-routing";
        public const string OrderCommandRouting = "order-command-routing";
    }
}
