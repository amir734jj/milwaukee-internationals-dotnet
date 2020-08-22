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
using static Models.Constants.GlobalConfigs;
using static Models.Constants.ApplicationConstants;

namespace Logic
{
    public class ConfigLogic : IConfigLogic
    {
        private readonly IS3Service _s3Service;

        private readonly ILogger<ConfigLogic> _logger;
        
        private readonly GlobalConfigs _globalConfigs;

        public ConfigLogic(IS3Service s3Service, ILogger<ConfigLogic> logger, GlobalConfigs globalConfigs)
        {
            _s3Service = s3Service;
            _logger = logger;
            _globalConfigs = globalConfigs;
        }

        public async Task<GlobalConfigViewModel> ResolveGlobalConfig()
        {
            var currentYear = DateTime.UtcNow.Year;

            var years = new HashSet<int> {2018, 2019, 2020, currentYear};

            var retVal = new GlobalConfigViewModel
            {
                Years = years,
                UpdatedYear = _globalConfigs.YearValue,
                EventFeature = _globalConfigs.EventFeature,
                EmailTestMode = _globalConfigs.EmailTestMode,
                Theme = _globalConfigs.CurrentTheme
            };

            return await Task.FromResult(retVal);
        }

        public async Task SetGlobalConfig(GlobalConfigViewModel globalConfigViewModel)
        {
            _globalConfigs.UpdateGlobalConfigs(globalConfigViewModel);

            await _s3Service.Upload(ConfigFile, globalConfigViewModel.ToByteArray(), new Dictionary<string, string>
            {
                ["Description"] = "Application config file"
            });
        }

        public async Task Refresh()
        {
            var response = await _s3Service.Download(ConfigFile);

            if (response.Status == HttpStatusCode.OK)
            {
                _logger.LogInformation("Successfully fetched the config from S3");
                
                var globalConfigViewModel = response.Data.Deserialize<GlobalConfigViewModel>();
                
                _globalConfigs.UpdateGlobalConfigs(globalConfigViewModel);
            }
            else
            {
                _logger.LogError("Failed to fetch the config from S3");
            }
        }
    }
}