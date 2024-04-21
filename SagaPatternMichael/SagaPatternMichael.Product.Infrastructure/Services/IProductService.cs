using Microsoft.EntityFrameworkCore;
using SagaPatternMichael.Product.Infrastructure.Core.Entities;
using SagaPatternMichael.Product.Infrastructure.DTOs;

namespace SagaPatternMichael.Product.Infrastructure.Services
{
    public interface IProductService
    {
        Task UpdateProduct(OrderDTO orderDTO);
    }

    public class ProductService : IProductService
    {
        // Sorry, I need to do that, this is inevitable
        private readonly ShopPartTimeContext _context;

        public ProductService(ShopPartTimeContext shopPartTimeContext)
        {
            _context = shopPartTimeContext;
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
