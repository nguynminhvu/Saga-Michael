namespace SagaPatternMichael.Order.Core.Entities;

public partial class Order
{
    public Guid Id { get; set; }

    public double Amount { get; set; }

    ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();    
    public DateTime CreatedOn { get; set; }

    public DateTime ModifiedOn { get; set; }

    private Order()
    {
        
    }
    protected Order(Guid id, double amount, DateTime createdOn, DateTime modifiedOn)
    {
        Id = Guid.NewGuid();
        Amount = amount;
        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
    }

    public static Order Create(double amount)
     => new Order(Guid.NewGuid(), amount, DateTime.Now, DateTime.Now);


}
