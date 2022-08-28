using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Data.Tables;
using DAL.Interfaces;
using Models.ViewModels.EventService;

namespace DAL.ServiceApi;

public class ApiEventService : IApiEventService
{
    private readonly TableServiceClient _tableServiceClient;

    private const string TableName = "event";

    public ApiEventService(TableServiceClient tableServiceClient)
    {
        _tableServiceClient = tableServiceClient;
    }

    public async Task RecordEvent(string description)
    {
        await _tableServiceClient.GetTableClient(TableName).AddEntityAsync(new ApiEvent
        {
            Description = description,
            RecordedDate = DateTimeOffset.Now,
            // This is needed to make sure events are added in reverse chronological order because azure blob storage doesn't have order by feature
            RowKey = (DateTime.MaxValue.Ticks - DateTimeOffset.Now.Ticks).ToString("d19"),
            PartitionKey = Guid.NewGuid().ToString()
        });
    }

    public async Task<IEnumerable<ApiEvent>> GetEvents(int limit = 20)
    {
        var pages = _tableServiceClient.GetTableClient(TableName).QueryAsync<ApiEvent>(maxPerPage: limit);
        
        await foreach (var page in pages.AsPages())
        {
            return page.Values.OrderByDescending(x => x.RecordedDate).ToList();
        }

        return new List<ApiEvent>();
    }
}