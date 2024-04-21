using SagaPatternMichael.Orchestration.Helpers;
using SagaPatternMichael.Orchestration.Models;

namespace SagaPatternMichael.Orchestration.Events
{
    public class OrderCompletedEvent
    {
        private readonly MessageSupport _messageSupport;
        private const string Queue = "InventoryQueue";
        private const string Exchange = "InventoryExchange";
        private const string RoutingKey = "inventory-routing-key";


        public OrderCompletedEvent(MessageSupport messageSupport)
        {
            _messageSupport = messageSupport;
        }

        public async Task SendEvent(MessageDTO messageDTO)
        {
            //Some business logic
            await _messageSupport.SendMessage(messageDTO, Queue, Exchange, RoutingKey);
        }
    }
}
