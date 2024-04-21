namespace SagaPatternMichael.Orchestration.Data;

public partial class EventErrorBox
{
    public Guid Id { get; set; }

    public string Data { get; set; } = null!;

    public DateTime ModifiedOn { get; set; }

    public DateTime CreatedOn { get; set; }

    protected EventErrorBox(Guid id, string data, DateTime modifiedon, DateTime createdon)
    {
        Id = id;
        Data = data;
        ModifiedOn = modifiedon;
        CreatedOn = createdon;
    }

   public static EventErrorBox Create(string data) => new EventErrorBox(Guid.NewGuid(), data, DateTime.Now, DateTime.Now);
}
