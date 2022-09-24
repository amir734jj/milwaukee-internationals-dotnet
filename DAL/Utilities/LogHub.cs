using DAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DAL.Utilities;

[AllowAnonymous]
public class LogHub : Hub
{
    private readonly ILogger<LogHub> _logger;
    private readonly IApiEventService _apiEventService;

    public LogHub(ILogger<LogHub> logger, IApiEventService apiEventService)
    {
        _logger = logger;
        _apiEventService = apiEventService;
    }
    
    public void Sink(object props)
    {
        _logger.LogTrace("Mobile app log: {}", props);

        _apiEventService.RecordEvent(JsonConvert.SerializeObject(props));
    }
}