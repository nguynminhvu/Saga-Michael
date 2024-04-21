using System;
using System.Collections.Generic;

namespace SagaPatternMichael.Payment.Infrastructure.Core.Entities;

public partial class Payment
{
    public Guid Id { get; set; }

    public double Amount { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime ModifiedOn { get; set; }
}
