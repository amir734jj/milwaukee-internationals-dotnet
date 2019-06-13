using System.Threading.Tasks;
using Models.ViewModels;

namespace Logic.Interfaces
{
    public interface IConfigLogic
    {
        Task<YearContextViewModel> ResolveYearContext();

        Task SetYearContext(int year);
    }
}