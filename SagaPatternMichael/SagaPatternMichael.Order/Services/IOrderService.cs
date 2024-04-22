using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SagaPatternMichael.Order.Core.Entities;
using SagaPatternMichael.Order.DTOs;
using SagaPatternMichael.Order.RabbitMQ.Events;
using SagaPatternMichael.Order.UnitOfWork;

namespace SagaPatternMichael.Order.Services
{
    public interface IOrderService
    {
        Task CheckOut(CartDTO cartDTO);
        Task AddEvent(EventBox eventBox);
        Task AddCommand(CommandBox commandBox);
        Task<List<EventBox>> GetEvents();
        Task<List<CommandBox>> GetCommands();
        Task RemoveEvent(EventBox eventBox);
        Task RemoveCommand(CommandBox commandBox);
        Task UpdateOrder(Order.Core.Entities.Order order);
    }

    public class OrderService : IOrderService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _uow;

        public OrderService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _configuration = configuration;
            _uow = unitOfWork;
        }

        public async Task AddCommand(CommandBox commandBox)
        {
            await _uow.CommandBoxRepository.AddAsync(commandBox);
            await _uow.SaveChangesAsync();
        }

        public async Task AddEvent(EventBox eventBox)
        {
            await _uow.EventBoxRepository.AddAsync(eventBox);
            await _uow.SaveChangesAsync();
        }

        public async Task CheckOut(CartDTO cartDTO)
        {
            double amount = 0;
            foreach (var item in cartDTO.ProductDTOs)
            {
                amount += item.Price * item.Quantity;
            }
            var order = Order.Core.Entities.Order.Create(amount);
            await _uow.OrderRepository.AddAsync(order);
            List<OrderLineDTO> orderLines = new();

            foreach (var item in cartDTO.ProductDTOs)
            {
                var orderLine = OrderLine.Create(order.Id, item.Price, item.Quantity, item.Id);
                orderLines.Add(new OrderLineDTO
                {
                    Id = orderLine.Id,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    OrderId = order.Id,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    ProductId = item.Id
                });
                await _uow.OrderLineRepository.AddAsync(orderLine);
            }
            OrderDTO orderDTO = new OrderDTO
            {
                Id = order.Id,
                Amount = amount,
                ModifiedOn = DateTime.Now,
                CreatedOn = DateTime.Now,
                OrderLines = orderLines
            };
            var rs = await _uow.SaveChangesAsync();
            if (rs > 0)
            {
                // raise event
                OrderCompletedEvent orderCompletedEvent = new OrderCompletedEvent(_configuration);
                await orderCompletedEvent.SendMessage(new MessageDTO
                {
                    Data = JsonConvert.SerializeObject(orderDTO),
                    Source = "OrderCompletedEvent"
                }, OrchestrationQueue.OrchestrationEvent, OrchestrationExchange.OrchestrationEvent, OrchestrationRoutingKey.OrchestrationEvent);
            }
            else
            {
                // roll
                OrderErrorEvent orderErrorEvent = new OrderErrorEvent(_configuration);
                await orderErrorEvent.SendMessage(new MessageDTO
                {
                    Data = JsonConvert.SerializeObject(orderDTO),
                    Source = "OrderErrorEvent"
                }, OrchestrationQueue.OrchestrationErrorEvent, OrchestrationExchange.OrchestrationErrorEvent, OrchestrationRoutingKey.OrchestrationErrorEvent);
            }
        }

        public async Task<List<CommandBox>> GetCommands()
          => await _uow.CommandBoxRepository.GetAll().ToListAsync();

        public async Task<List<EventBox>> GetEvents()
          => await _uow.EventBoxRepository.GetAll().ToListAsync();

        public async Task RemoveCommand(CommandBox commandBox)
        {
            _uow.CommandBoxRepository.Remove(commandBox);
            await _uow.SaveChangesAsync();
        }

        public async Task RemoveEvent(EventBox eventBox)
        {
            _uow.EventBoxRepository.Remove(eventBox);
            await _uow.SaveChangesAsync();
        }

        public async Task UpdateOrder(Core.Entities.Order order)
        {
            _uow.OrderRepository.Update(order);
            await _uow.SaveChangesAsync();
        }
    }
}
