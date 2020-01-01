using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using DAL.Extensions;
using DAL.Interfaces;
using Logic.Interfaces;
using Models.ViewModels.Config;
using static Models.Constants.GlobalConfigs;
using static Models.Constants.ApplicationConstants;

namespace Logic
{
    public class ConfigLogic : IConfigLogic
    {
        private readonly IS3Service _s3Service;

        public ConfigLogic(IS3Service s3Service)
        {
            _s3Service = s3Service;
        }

        public async Task<GlobalConfigViewModel> ResolveGlobalConfig()
        {
            var currentYear = DateTime.UtcNow.Year;

            var years = new HashSet<int> {2018, 2019, 2020, currentYear};

            var retVal = new GlobalConfigViewModel
            {
                Years = years,
                UpdatedYear = YearValue,
                EventFeature = EventFeature,
                EmailTestMode = EmailTestMode,
                Theme = CurrentTheme
            };

            return await Task.FromResult(retVal);
        }

        public async Task SetGlobalConfig(GlobalConfigViewModel globalConfigViewModel)
        {
            UpdateGlobalConfigs(globalConfigViewModel);

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
                var globalConfigViewModel = response.Data.Deserialize<GlobalConfigViewModel>();
                
                UpdateGlobalConfigs(globalConfigViewModel);
            }
        }
    }
}