using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Entities;

namespace DAL.Interfaces;

public interface IConfigLogic
{
    Task<GlobalConfigs> ResolveGlobalConfig();

    Task SetGlobalConfig(GlobalConfigs globalConfigs);

    IEnumerable<int> GetYears();
}