using System;
using System.Collections.Generic;

namespace SagaPatternMichael.Product.Infrastructure.Core.Entities;

public partial class EventErrorBox
{
    public Guid Id { get; set; }

    public string Data { get; set; } = null!;

    public DateTime ModifiedOn { get; set; }

    public DateTime CreatedOn { get; set; }
}
