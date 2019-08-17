using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Logic.Interfaces;
using Models.Constants;
using Models.ViewModels;
using Models.ViewModels.Config;

namespace Logic
{
    public class ConfigLogic : IConfigLogic
    {
        public async Task<GlobalConfigViewModel> ResolveGlobalConfig()
        {
            var currentYear = DateTime.UtcNow.Year;

            var years = new HashSet<int> {2018, 2019, currentYear};

            var retVal = new GlobalConfigViewModel
            {
                Years = years,
                UpdatedYear = GlobalConfigs.YearValue,
                EventFeature = GlobalConfigs.EventFeature,
                EmailTestMode = GlobalConfigs.EmailTestMode
            };
            
            return await Task.FromResult(retVal);
        }

        public Task SetGlobalConfig(GlobalConfigViewModel globalConfigViewModel)
        {
            GlobalConfigs.UpdateGlobalConfigs(globalConfigViewModel);

            return Task.CompletedTask;
        }
    }
}