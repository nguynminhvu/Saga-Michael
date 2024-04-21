namespace SagaPatternMichael.Product.Infrastructure.DTOs
{
    public class OrderLineDTO
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public Guid ProductId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
