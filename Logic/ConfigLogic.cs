using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using DAL.Extensions;
using DAL.Interfaces;
using Logic.Interfaces;
using Microsoft.Extensions.Logging;
using Models.Constants;
using Models.ViewModels.Config;
using static Models.Constants.ApplicationConstants;

namespace Logic
{
    public class ConfigLogic : IConfigLogic
    {
        /// <summary>
        /// This is the year when milwaukee-internationals started
        /// </summary>
        private const int StartYear = 2017; // DO-NOT CHANGE!

        private readonly IStorageService _storageService;

        private readonly ILogger<ConfigLogic> _logger;
        
        private readonly GlobalConfigs _globalConfigs;

        public ConfigLogic(IStorageService storageService, ILogger<ConfigLogic> logger, GlobalConfigs globalConfigs)
        {
            _storageService = storageService;
            _logger = logger;
            _globalConfigs = globalConfigs;
        }

        public async Task<GlobalConfigViewModel> ResolveGlobalConfig()
        {
            var years = new HashSet<int>();
            var currentYear = StartYear;
            
            while (currentYear <= DateTime.UtcNow.Year)
            {
                years.Add(currentYear++);
            }

            var retVal = new GlobalConfigViewModel
            {
                Years = years,
                UpdatedYear = _globalConfigs.YearValue,
                EventFeature = _globalConfigs.EventFeature,
                EmailTestMode = _globalConfigs.EmailTestMode,
                Theme = _globalConfigs.CurrentTheme,
                DisallowDuplicateStudents = _globalConfigs.DisallowDuplicateStudents
            };

            return await Task.FromResult(retVal);
        }

        public async Task SetGlobalConfig(GlobalConfigViewModel globalConfigViewModel)
        {
            _globalConfigs.UpdateGlobalConfigs(globalConfigViewModel);

            await _storageService.Upload(ConfigFile, globalConfigViewModel.ToByteArray(), new Dictionary<string, string>
            {
                ["Description"] = "Application config file"
            });
        }

        public async Task Refresh()
        {
            var response = await _storageService.Download(ConfigFile);

            if (response.Status == HttpStatusCode.OK)
            {
                _logger.LogInformation("Successfully fetched the config from storage service");
                
                var globalConfigViewModel = response.Data.Deserialize<GlobalConfigViewModel>() ?? new GlobalConfigViewModel();
                
                _globalConfigs.UpdateGlobalConfigs(globalConfigViewModel);
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
}