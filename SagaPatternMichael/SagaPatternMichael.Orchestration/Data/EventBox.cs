using System;
using System.Collections.Generic;

namespace SagaPatternMichael.Orchestration.Data;

public  class EventBox
{
    public Guid Id { get; set; }

    public string Data { get; set; } = null!;

    public DateTime ModifiedOn { get; set; }

    public DateTime CreatedOn { get; set; }
    public EventBox()
    {
        
    }
    protected EventBox(Guid id, string data, DateTime modifiedon, DateTime createdon)
    {
        Id = id;
        Data = data;
        ModifiedOn = modifiedon;
        CreatedOn = createdon;
    }

   public static EventBox Create(string data) => new EventBox(Guid.NewGuid(), data, DateTime.Now, DateTime.Now);
}
