using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Logic.Interfaces;
using Models.Constants;
using Models.ViewModels;

namespace Logic
{
    public class ConfigLogic : IConfigLogic
    {
        public async Task<YearContextViewModel> ResolveYearContext()
        {
            var years = new List<int> {2018, 2019};

            var retVal = new YearContextViewModel
            {
                Years = years.ToList()
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