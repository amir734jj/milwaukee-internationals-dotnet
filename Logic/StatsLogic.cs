using System;
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
            var students = (await _studentDal.GetAll(x => x.Year == year)).ToList();
            
            var countDrivers = await _driverDal.Count(x => x.Year == year && x.Role == RolesEnum.Driver);
            var countNavigators = await _driverDal.Count(x => x.Year == year && x.Role == RolesEnum.Navigator);
            var countHosts = await _hostDal.Count(x => x.Year == year);
            var countDependents = students.Select(x => x.FamilySize).Sum();
            var countDistinctCountries = students.Select(x => x.Country.ToLower()).Distinct().Count();
            
            var currentYear = DateTime.Now.Year == year;
            var activeYear = countDrivers > 0;
            
            result.Add(new StatsViewModel
            {
                Year = year,
                CountDrivers = countDrivers,
                CountNavigators = countNavigators,
                CountStudents = students.Count,
                CountHosts = countHosts,
                CountDependents = countDependents,
                CountDistinctCountries = countDistinctCountries,
                CurrentYear = currentYear,
                ActiveYear = activeYear
            }); 
        }

        return result;
    }
}