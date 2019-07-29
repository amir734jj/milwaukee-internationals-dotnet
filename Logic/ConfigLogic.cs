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
        public async Task<YearContextViewModel> ResolveYearContext()
        {
            var currentYear = DateTime.Now.Date.Year;

            var years = new HashSet<int> {2018, 2019, currentYear};

            var retVal = new YearContextViewModel
            {
                Years = years,
                UpdatedYear = YearContext.YearValue
            };
            
            return await Task.FromResult(retVal);
        }

        public Task SetYearContext(int year)
        {
            YearContext.YearValue = year;

            return Task.CompletedTask;
        }
    }
}