using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Data.Tables;
using DAL.Interfaces;
using Models.Constants;
using Models.ViewModels.EventService;

namespace DAL.ServiceApi;

public class ApiEventService : IApiEventService
{
    private readonly TableServiceClient _tableServiceClient;
    private readonly GlobalConfigs _globalConfigs;

    private const string TableName = "event";

    private const int QueryLimit = 30;

    public ApiEventService(TableServiceClient tableServiceClient, GlobalConfigs globalConfigs)
    {
        _tableServiceClient = tableServiceClient;
        _globalConfigs = globalConfigs;
    }

    public async Task RecordEvent(string description)
    {
        if (!_globalConfigs.RecordApiEvents) return;
        
        await _tableServiceClient.GetTableClient(TableName).AddEntityAsync(new ApiEvent
        {
            Description = description,
            RecordedDate = DateTimeOffset.Now,
            // This is needed to make sure events are added in reverse chronological order because azure blob storage doesn't have order by feature
            RowKey = (DateTime.MaxValue.Ticks - DateTimeOffset.Now.Ticks).ToString("d19"),
            PartitionKey = Guid.NewGuid().ToString()
        });
    }

    public async Task<IEnumerable<ApiEvent>> GetEvents()
    {
        return await GetEvents(limit: QueryLimit);
    }

    private async Task<IEnumerable<ApiEvent>> GetEvents(int limit)
    {
        var pages = _tableServiceClient.GetTableClient(TableName).QueryAsync<ApiEvent>(maxPerPage: limit);
        
        await foreach (var page in pages.AsPages())
        {
            return page.Values.OrderByDescending(x => x.RecordedDate).ToList();
        }

        return new List<ApiEvent>();
    }
}