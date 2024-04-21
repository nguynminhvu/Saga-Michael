using SagaPatternMichael.Order.Core.Entities;
using SagaPatternMichael.Order.Repositories;

namespace SagaPatternMichael.Order.UnitOfWork
{
    public interface IUnitOfWork
    {
        IRepository<Order.Core.Entities.Order> OrderRepository { get; }
        IRepository<Order.Core.Entities.OrderLine> OrderLineRepository { get; }
        Task<int> SaveChangesAsync();
    }
    public class UnitOfWork : IUnitOfWork
    {
        private readonly OrderServiceContext _context;

        public UnitOfWork(OrderServiceContext orderServiceContext)
        {
            _context = orderServiceContext;
            OrderRepository = new Repository<Order.Core.Entities.Order>(_context);
            OrderLineRepository = new Repository<Order.Core.Entities.OrderLine>(_context);
        }
        public IRepository<Core.Entities.Order> OrderRepository { get; private set; }

        public IRepository<OrderLine> OrderLineRepository { get; private set; }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
