using System;
using System.Collections.Generic;

namespace SagaPatternMichael.Order.Core.Entities;

public partial class OrderLine
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    public Guid ProductId { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }

    private OrderLine()
    {

    }
    protected OrderLine(Guid id, Guid orderId, double price, int quantity, Guid productId, DateTime createdOn, DateTime modifiedOn)
    {
        Id = id;
        OrderId = orderId;
        Price = price;
        Quantity= quantity;
        ProductId = productId;
        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
    }

    public static OrderLine Create(Guid orderId, double price,int quantity, Guid productId)
        => new OrderLine(Guid.NewGuid(), orderId, price, quantity, productId, DateTime.Now, DateTime.Now);
}
