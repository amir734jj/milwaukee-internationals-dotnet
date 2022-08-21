using System.Collections.Generic;
using System.Threading.Tasks;
using Models.ViewModels;

namespace Logic.Interfaces;

public interface IStatsLogic
{
    Task<List<StatsViewModel>> GetStats();
}