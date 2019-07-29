using System.Threading.Tasks;
using Models.ViewModels;
using Models.ViewModels.Config;

namespace Logic.Interfaces
{
    public interface IConfigLogic
    {
        Task<YearContextViewModel> ResolveYearContext();

        Task SetYearContext(int year);
    }
}