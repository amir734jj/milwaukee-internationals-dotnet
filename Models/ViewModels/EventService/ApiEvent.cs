using System;
using Azure;
using Azure.Data.Tables;

namespace Models.ViewModels.EventService;

// C# record type for items in the table
public record ApiEvent : ITableEntity
{
    public string RowKey { get; set; }

    public string PartitionKey { get; set; } = default!;
    
    public DateTimeOffset RecordedDate { get; set; }

    public string Description { get; set; }

    public ETag ETag { get; set; } = default!;

    public DateTimeOffset? Timestamp { get; set; } = default!;
}