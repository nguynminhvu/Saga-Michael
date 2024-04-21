namespace SagaPatternMichael.Product.Infrastructure.Core.Entities;

public partial class Product
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Desciption { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public bool IsActived { get; set; }

    public Guid CategoryId { get; set; }

    public long Version { get; set; }

    public virtual Category Category { get; set; } = null!;
}
