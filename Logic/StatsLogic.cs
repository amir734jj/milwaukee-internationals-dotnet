using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EfCoreRepository.Interfaces;
using Logic.Interfaces;
using Models.Entities;
using Models.Enums;
using Models.ViewModels;

namespace Logic;

public class StatsLogic : IStatsLogic
{
    private readonly IConfigLogic _configLogic;
    private readonly IBasicCrud<Student> _studentDal;
    private readonly IBasicCrud<Driver> _driverDal;
    private readonly IBasicCrud<Host> _hostDal;

    public StatsLogic(IEfRepository repository, IConfigLogic configLogic)
    {
        _configLogic = configLogic;
        _studentDal = repository.For<Student>();
        _driverDal = repository.For<Driver>();
        _hostDal = repository.For<Host>();
    }
    
    public async Task<List<StatsViewModel>> GetStats()
    {
        var result = new List<StatsViewModel>();
        
        foreach (var year in _configLogic.GetYears())
        {
            result.Add(new StatsViewModel
            {
                Year = year,
                CountDrivers = await _driverDal.Count(x => x.Year == year && x.Role == RolesEnum.Driver),
                CountNavigators = await _driverDal.Count(x => x.Year == year && x.Role == RolesEnum.Navigator),
                CountStudents = await _studentDal.Count(x => x.Year == year),
                CountHosts = await _hostDal.Count(x => x.Year == year),
                CountDependents = (await _studentDal.GetAll(x => x.Year == year)).Select(x => x.FamilySize).Sum()
            }); 
        }

        return result;
    }
}