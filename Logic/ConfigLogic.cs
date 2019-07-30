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
                UpdatedYear = YearContext.YearValue,
                EventFeature = GlobalConfigs.EventFeature
            };
            
            return await Task.FromResult(retVal);
        }

        public Task SetGlobalConfig(GlobalConfigViewModel globalConfigViewModel)
        {
            YearContext.YearValue = globalConfigViewModel.UpdatedYear;

            GlobalConfigs.EventFeature = globalConfigViewModel.EventFeature;
            
            return Task.CompletedTask;
        }
    }
}