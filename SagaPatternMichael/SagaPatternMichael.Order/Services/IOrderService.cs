using SagaPatternMichael.Order.Core.Entities;
using SagaPatternMichael.Order.DTOs;
using SagaPatternMichael.Order.UnitOfWork;

namespace SagaPatternMichael.Order.Services
{
    public interface IOrderService
    {
        Task CheckOut(CartDTO cartDTO);
    }

    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _uow;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _uow = unitOfWork;
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
    }
}
