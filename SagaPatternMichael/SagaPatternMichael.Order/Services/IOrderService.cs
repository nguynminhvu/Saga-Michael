using Microsoft.EntityFrameworkCore;
using SagaPatternMichael.Order.Core.Entities;
using SagaPatternMichael.Order.DTOs;
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
        private readonly IUnitOfWork _uow;

        public OrderService(IUnitOfWork unitOfWork)
        {
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

            foreach (var item in cartDTO.ProductDTOs)
            {
                await _uow.OrderLineRepository.AddAsync(OrderLine.Create(order.Id, item.Price, item.Quantity, item.Id));
            }
            var rs = await _uow.SaveChangesAsync();
            if (rs > 0)
            {
                // raise event
            }
            else
            {
                // roll
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
