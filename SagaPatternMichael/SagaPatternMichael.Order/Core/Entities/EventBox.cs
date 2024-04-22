namespace SagaPatternMichael.Order.Core.Entities;

public partial class EventBox
{
    public Guid Id { get; set; }
    public string Data { get; set; } = null!;

    public DateTime ModifiedOn { get; set; }

    public DateTime CreatedOn { get; set; }
}
