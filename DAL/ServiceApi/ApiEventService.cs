using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Data.Tables;
using DAL.Interfaces;
using DAL.Utilities;
using Microsoft.AspNetCore.SignalR;
using Models.Constants;
using Models.ViewModels.EventService;

namespace DAL.ServiceApi;

public class ApiEventService : IApiEventService
{
    private readonly TableServiceClient _tableServiceClient;
    private readonly GlobalConfigs _globalConfigs;
    private readonly IHubContext<MessageHub> _hubContext;

    private const string TableName = "event";

    private const int QueryLimit = 35;

    public ApiEventService(TableServiceClient tableServiceClient, GlobalConfigs globalConfigs, IHubContext<MessageHub> hubContext)
    {
        _tableServiceClient = tableServiceClient;
        _globalConfigs = globalConfigs;
        _hubContext = hubContext;
    }

    public async Task RecordEvent(string description)
    {
        var entity = new ApiEvent
        {
            Description = description,
            RecordedDate = DateTimeOffset.Now,
            // This is needed to make sure events are added in reverse chronological order because azure blob storage doesn't have order by feature
            RowKey = (DateTimeOffset.MaxValue.Ticks - DateTimeOffset.Now.Ticks).ToString("d19"),
            PartitionKey = DateTimeOffset.Now.ToString("yy-MM-dd")
        };

        await _hubContext.Clients.All.SendCoreAsync("events", new object[] { entity});

        if (_globalConfigs.RecordApiEvents)
        {
            await _tableServiceClient.GetTableClient(TableName).AddEntityAsync(entity);
        }
    }

    public async Task<IEnumerable<ApiEvent>> GetEvents()
    {
        return await GetEvents(limit: QueryLimit);
    }

    private async Task<IEnumerable<ApiEvent>> GetEvents(int limit)
    {
        var pages = _tableServiceClient.GetTableClient(TableName).QueryAsync<ApiEvent>(x => x.Timestamp > DateTimeOffset.Now.Subtract(TimeSpan.FromDays(3)), maxPerPage: limit);
        
        await foreach (var page in pages.AsPages())
        {
            return page.Values.ToList();
        }

        return new List<ApiEvent>();
    }
}