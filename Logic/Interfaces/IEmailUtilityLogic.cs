using System.Threading.Tasks;
using Models.ViewModels;

namespace Logic.Interfaces
{
    public interface IEmailUtilityLogic
    {
        Task<bool> Handle(EmailFormViewModel emailFormViewModel);
    }
}