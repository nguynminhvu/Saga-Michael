using System;
using System.Collections.Generic;

namespace SagaPatternMichael.Order.Core.Entities;

public partial class Order
{
    public Guid Id { get; set; }

    public double Amount { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public DateTime ModifiedOn { get; set; }

    private Order()
    {

    }
    protected Order(Guid id, double amount, string status, DateTime createdOn, DateTime modifiedOn)
    {
        Id = Guid.NewGuid();
        Amount = amount;
        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
    }

    public static Order Create(double amount)
     => new Order(Guid.NewGuid(), amount, "Process", DateTime.Now, DateTime.Now);

    public void Update(string status)
    {
        Status = status;
        ModifiedOn = DateTime.Now;
    }
}
