using Microsoft.AspNetCore.Mvc;
using SagaPatternMichael.Order.DTOs;
using SagaPatternMichael.Order.Services;

namespace SagaPatternMichael.Order.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CheckOut()
        {
            List<ProductDTO> productDTOs = new List<ProductDTO>();
            productDTOs.Add(new ProductDTO
            {
                Id = Guid.Parse("FB2C0172-C893-46C8-938C-E52422D6A5AF"),
                Price = 50,
                Quantity = 2
            });
            CartDTO cartDTO = new CartDTO { ProductDTOs = productDTOs };

            await _orderService.CheckOut(cartDTO);
            return Created();
        }
    }
}
