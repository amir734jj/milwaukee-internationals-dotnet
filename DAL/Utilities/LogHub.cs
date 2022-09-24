using System.Threading.Tasks;
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
    
    public async Task Sink(object props)
    {
        await _apiEventService.RecordEvent($"Mobile app log: {JsonConvert.SerializeObject(props)}");
    }
}