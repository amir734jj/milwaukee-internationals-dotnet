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
        private readonly IStudentLogic _studentLogic;
        private readonly IDriverLogic _driverLogic;
        private readonly IHostLogic _hostLogic;

        public ConfigLogic(IStudentLogic studentLogic, IDriverLogic driverLogic, IHostLogic hostLogic)
        {
            _studentLogic = studentLogic;
            _driverLogic = driverLogic;
            _hostLogic = hostLogic;
        }
        
        public async Task<YearContextViewModel> ResolveYearContext()
        {
            var years = (await _studentLogic.GetAll()).Select(x => x.Year)
                .Concat((await _driverLogic.GetAll()).Select(x => x.Year))
                .Concat((await _hostLogic.GetAll()).Select(x => x.Year))
                .Distinct();

            return new YearContextViewModel
            {
                Years = years.ToList()
            };
        }

        public Task SetYearContext(int year)
        {
            YearContext.YearValue = year;

            return Task.CompletedTask;
        }
    }
}