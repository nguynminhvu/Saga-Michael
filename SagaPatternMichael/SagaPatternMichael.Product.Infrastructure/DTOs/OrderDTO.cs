﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaPatternMichael.Product.Infrastructure.DTOs
{
    public class OrderDTO
    {
        public Guid Id { get; set; }
        public double Amount { get; set; }
        public DateTime CreatedOn { get; set; }
        public List<OrderLineDTO> OrderLines { get; set; }
        public DateTime ModifiedOn { get; set; }

    }
}
