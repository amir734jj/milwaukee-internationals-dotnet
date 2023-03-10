using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using DAL.Extensions;
using DAL.Interfaces;
using Logic.Interfaces;
using Microsoft.Extensions.Logging;
using Models.Constants;
using Models.ViewModels.Config;
using static Models.Constants.ApplicationConstants;

namespace Logic;

public class ConfigLogic : IConfigLogic
{
    /// <summary>
    /// This is the year when milwaukee-internationals started
    /// </summary>
    private const int StartYear = 2017; // DO-NOT CHANGE!

    private readonly IStorageService _storageService;
    private readonly ILogger<ConfigLogic> _logger;
    private readonly GlobalConfigs _globalConfigs;
    private readonly IApiEventService _apiEventService;
    private readonly IMapper _mapper;

    public ConfigLogic(IStorageService storageService, ILogger<ConfigLogic> logger, GlobalConfigs globalConfigs, IApiEventService apiEventService, IMapper mapper)
    {
        _storageService = storageService;
        _logger = logger;
        _globalConfigs = globalConfigs;
        _apiEventService = apiEventService;
        _mapper = mapper;
    }

    public GlobalConfigViewModel ResolveGlobalConfig()
    {
        var retVal = _mapper.Map<GlobalConfigViewModel>(_globalConfigs);

        return retVal;
    }

    public async Task SetGlobalConfig(GlobalConfigViewModel globalConfigViewModel)
    {
        _mapper.Map(globalConfigViewModel, _globalConfigs);

        await _storageService.Upload(ConfigFile, globalConfigViewModel.ToByteArray(), new Dictionary<string, string>
        {
            ["Description"] = "Application config file"
        });

        await _apiEventService.RecordEvent($"Handled updating of global config {_globalConfigs}");
    }

    public async Task Refresh()
    {
        var response = await _storageService.Download(ConfigFile);
            
        if (response.Status == HttpStatusCode.OK)
        {
            _logger.LogInformation("Successfully fetched the config from storage service");
                
            var globalConfigViewModel = response.Data.Deserialize<GlobalConfigViewModel>() ?? new GlobalConfigViewModel();

            _mapper.Map(globalConfigViewModel, _globalConfigs);
                
            await _apiEventService.RecordEvent($"Refreshed global config {_globalConfigs}");
        }
        else
        {
            _logger.LogError("Failed to fetch the config from storage service");
        }
    }

    public IEnumerable<int> GetYears()
    {
        var currentYear = StartYear;
        while (currentYear <= DateTime.Now.Year)
        {
            yield return currentYear;
            currentYear++;
        }
    }
}