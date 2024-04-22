
using Newtonsoft.Json;
using SagaPatternMichael.Order.DTOs;
using SagaPatternMichael.Order.Services;

namespace SagaPatternMichael.Order.RabbitMQ.Commands
{
    public class OrderPaymentedCommand : MessageSupport
    {
        private readonly IOrderService _orderService;

        public OrderPaymentedCommand(IConfiguration configuration, IOrderService orderService) : base(configuration)
        {
            _orderService = orderService;
        }

        public override string Queue => "OrderUpdatedQueue";

        public override string Exchange => "OrderUpdatedExchange";

        public override string RoutingKey => "order-updated-routing-key";

        public async Task UpdateOrder(MessageDTO messageDTO)
        {
            var order = JsonConvert.DeserializeObject<Core.Entities.Order>(messageDTO.Data);
            if (order != null!)
            {
                order.Update("Payment");
                await _orderService.UpdateOrder(order);
            }
        }
    }
}
