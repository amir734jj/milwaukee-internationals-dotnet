using System.Threading.Tasks;
using Models.ViewModels;

namespace Logic.Interfaces;

public interface ISmsUtilityLogic
{
    Task<bool> HandleAdHocSms(SmsFormViewModel smsFormViewModel);
    
    Task<SmsFormViewModel> GetSmsForm();
}