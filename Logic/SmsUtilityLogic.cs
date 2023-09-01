using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Interfaces;
using Logic.Interfaces;
using Models.Constants;
using Models.Enums;
using Models.ViewModels;
using Newtonsoft.Json;

namespace Logic;

public class SmsUtilityLogic : ISmsUtilityLogic
{
    private readonly GlobalConfigs _globalConfigs;
    private readonly ISmsService _smsService;
    private readonly IStudentLogic _studentLogic;
    private readonly IDriverLogic _driverLogic;
    private readonly IHostLogic _hostLogic;
    private readonly IUserLogic _userLogic;
    private readonly IEmailServiceApi _emailServiceApi;
    private readonly IApiEventService _apiEventService;
    private readonly IRegistrationLogic _registrationLogic;

    public SmsUtilityLogic(
        GlobalConfigs globalConfigs,
        ISmsService smsService,
        IStudentLogic studentLogic,
        IDriverLogic driverLogic,
        IHostLogic hostLogic,
        IUserLogic userLogic,
        IEmailServiceApi emailServiceApi,
        IApiEventService apiEventService,
        IRegistrationLogic registrationLogic)
    {
        _globalConfigs = globalConfigs;
        _smsService = smsService;
        _studentLogic = studentLogic;
        _driverLogic = driverLogic;
        _hostLogic = hostLogic;
        _userLogic = userLogic;
        _emailServiceApi = emailServiceApi;
        _apiEventService = apiEventService;
        _registrationLogic = registrationLogic;
    }
    
    public async Task<bool> HandleAdHocSms(SmsFormViewModel smsFormViewModel)
    {
        var year = _globalConfigs.YearValue;
            
        var phoneNumbers = new List<string>();

        // Add admin email
        if (smsFormViewModel.Admin)
        {
            var admins = (await _userLogic.GetAll()).Where(x => x.UserRoleEnum == UserRoleEnum.Admin)
                .Select(x => x.Email)
                .ToList();

            phoneNumbers.AddRange(admins);
        }

        // Add student emails
        if (smsFormViewModel.Students)
        {
            var students = await _studentLogic.GetAll(year);
            phoneNumbers.AddRange(students.Select(x => x.Email).Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        // Add driver emails
        if (smsFormViewModel.Drivers)
        {
            var drivers = await _driverLogic.GetAll(year);
            phoneNumbers.AddRange(drivers.Select(x => x.Email).Where(x => !string.IsNullOrWhiteSpace(x)));
        }
            
        // Add host emails
        if (smsFormViewModel.Hosts)
        {
            var hosts = await _hostLogic.GetAll(year);
            phoneNumbers.AddRange(hosts.Select(x => x.Email).Where(x => !string.IsNullOrWhiteSpace(x)));
        }
            
        // Add user emails
        if (smsFormViewModel.Users)
        {
            var users = await _userLogic.GetAll();
            phoneNumbers.AddRange(users.Select(x => x.Email).Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        // Add additional emails
        if (!string.IsNullOrWhiteSpace(smsFormViewModel.AdditionalRecipients))
        {
            var emails = smsFormViewModel.AdditionalRecipients.Split(',').Select(x => x.Trim()).ToList();
            phoneNumbers.AddRange(emails);
        }

        // Remove duplicates
        phoneNumbers = phoneNumbers.Distinct().ToList();

        // Send the email
        await _smsService.SendMessage(phoneNumbers, smsFormViewModel.Message);

        await _apiEventService.RecordEvent($"Sent ad-hoc SMS to {string.Join(',', phoneNumbers)}");

        return true;
    }

    public async Task<SmsFormViewModel> GetSmsForm()
    {
        var year = _globalConfigs.YearValue;
            
        return new SmsFormViewModel
        {
            AdminCount = (await _userLogic.GetAll()).Count(x => x.UserRoleEnum == UserRoleEnum.Admin),
            StudentCount = (await _studentLogic.GetAll(year)).Count(),
            DriverCount = (await _driverLogic.GetAll(year)).Count(),
            HostCount = (await _hostLogic.GetAll(year)).Count(),
            UserCount = (await _userLogic.GetAll()).Count()
        };
    }

    public async Task HandleDriverSms()
    {
        var year = _globalConfigs.YearValue;

        foreach (var driver in await _driverLogic.GetAll(year))
        {
            await _registrationLogic.SendDriverSms(driver);
        }
    }

    public async Task IncomingSms(IncomingSmsViewModel body)
    {
        // Ignore spam messages
        if (body?.data?.payload?.is_spam ?? true)
        {
            return;
        }
        
        var text = $"SMS from {body.data?.payload?.from?.phone_number} [{body.data?.payload?.from?.carrier}]\n" +
                   $"{body.data?.payload?.text}";
        
        await _emailServiceApi.SendEmailAsync(ApiConstants.SiteEmail, "SMS received", text);
    }
}