using Microsoft.EntityFrameworkCore;
using SagaPatternMichael.Payment.Infrastructure.Core.Entities;
using SagaPatternMichael.Payment.Infrastructure.DTOs;

namespace SagaPatternMichael.Payment.Infrastructure.Services
{
    public interface IPaymentService
    {
        Task Payment(OrderDTO orderDTO);
        Task<List<EventBox>> GetEvents();
        Task AddEvent(EventBox eventBox);
        Task RemoveEvent(EventBox eventBox);
    }

    public class PaymentService : IPaymentService
    {
        private readonly PaymentServiceContext _context;

        public PaymentService(PaymentServiceContext paymentServiceContext)
        {
            _context = paymentServiceContext;
        }

        public async Task AddEvent(EventBox eventBox)
        {
            await _context.EventBoxes.AddAsync(eventBox);
            await _context.SaveChangesAsync();
        }

        public async Task<List<EventBox>> GetEvents()
         => await _context.EventBoxes.ToListAsync();

        public async Task Payment(OrderDTO orderDTO)
        {
            if (orderDTO == null!) throw new Exception("OrderDTO null!");
            var payment = new Payment.Infrastructure.Core.Entities.Payment
            {
                Amount = orderDTO.Amount,
                CreatedOn = DateTime.Now,
                Id = Guid.NewGuid(),
                ModifiedOn = DateTime.Now
            };
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveEvent(EventBox eventBox)
        {
            _context.EventBoxes.Remove(eventBox);
            await _context.SaveChangesAsync();
        }
    }
}
