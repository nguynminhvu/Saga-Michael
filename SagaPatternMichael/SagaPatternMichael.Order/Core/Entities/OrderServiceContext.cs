using Microsoft.EntityFrameworkCore;

namespace SagaPatternMichael.Order.Core.Entities;

public partial class OrderServiceContext : DbContext
{
    public OrderServiceContext()
    {
    }

    public OrderServiceContext(DbContextOptions<OrderServiceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderLine> OrderLines { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=nguynminhvu;Initial Catalog=OrderService;Integrated Security=True;Encrypt=False;Trust Server Certificate=True");

}
