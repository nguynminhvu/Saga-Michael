using System;
using System.Collections.Generic;

namespace SagaPatternMichael.Product.Infrastructure.Core.Entities;

public partial class Category
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsActived { get; set; }

    public long Version { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
