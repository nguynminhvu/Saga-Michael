using Microsoft.EntityFrameworkCore;
using SagaPatternMichael.Product.Infrastructure.Core.Entities;
using SagaPatternMichael.Product.Infrastructure.DTOs;

namespace SagaPatternMichael.Product.Infrastructure.Services
{
    public interface IProductService
    {
        Task UpdateProduct(OrderDTO orderDTO);
        Task<List<EventBox>> GetEvents();
        Task<List<EventErrorBox>> GetEventErrors();
        Task AddEvent(EventBox eventBox);
        Task AddEventError(EventErrorBox eventBox);
        Task RemoveEvent(EventBox eventBox);
        Task RemoveEventError(EventErrorBox eventBox);
        Task RollBackProduct(OrderDTO orderDTO);

    }

    public class ProductService : IProductService
    {
        private readonly ShopPartTimeContext _context;

        public ProductService(ShopPartTimeContext shopPartTimeContext)
        {
            _context = shopPartTimeContext;
        }

        public async Task AddEvent(EventBox eventBox)
        {
            await _context.EventBoxes.AddAsync(eventBox);
            await _context.SaveChangesAsync();
        }

        public async Task AddEventError(EventErrorBox eventBox)
        {
            await _context.EventErrorBoxes.AddAsync(eventBox);
            await _context.SaveChangesAsync();
        }

        public async Task<List<EventErrorBox>> GetEventErrors()
            => await _context.EventErrorBoxes.ToListAsync();


        public async Task<List<EventBox>> GetEvents()
            => await _context.EventBoxes.ToListAsync();

        public async Task RemoveEvent(EventBox eventBox)
        {
            _context.EventBoxes.Remove(eventBox);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveEventError(EventErrorBox eventBox)
        {
            _context.EventErrorBoxes.Remove(eventBox);
            await _context.SaveChangesAsync();
        }

        public async Task RollBackProduct(OrderDTO orderDTO)
        {
            if (orderDTO == null) throw new Exception("OrderDTO null !");

            foreach (var item in orderDTO.OrderLines)
            {
                var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == item.ProductId);
                if (product != null)
                {
                    product.Quantity = 0;
                    _context.Update(product);
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProduct(OrderDTO orderDTO)
        {
            if (orderDTO == null) throw new Exception("OrderDTO null !");

            foreach (var item in orderDTO.OrderLines)
            {
                var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == item.ProductId);
                if (product != null)
                {
                    product.Quantity = product.Quantity - item.Quantity;
                    _context.Update(product);
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
