using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Interfaces;
using EnumsNET;
using Logic.Interfaces;
using Models.Constants;
using Models.Enums;
using Models.ViewModels;

namespace Logic;

public class EmailUtilityLogic :  IEmailUtilityLogic
{
    private readonly GlobalConfigs _globalConfigs;
    private readonly IUserLogic _userLogic;
    private readonly IEmailServiceApi _emailServiceApiApi;
    private readonly IApiEventService _apiEventService;
    private readonly IStudentLogic _studentLogic;
    private readonly IHostLogic _hostLogic;
    private readonly IDriverLogic _driverLogic;

    /// <summary>
    /// Constructor dependency injection
    /// </summary>
    /// <param name="globalConfigs"></param>
    /// <param name="studentLogic"></param>
    /// <param name="driverLogic"></param>
    /// <param name="hostLogic"></param>
    /// <param name="userLogic"></param>
    /// <param name="emailServiceApiApi"></param>
    /// <param name="apiEventService"></param>
    public EmailUtilityLogic(GlobalConfigs globalConfigs, IStudentLogic studentLogic, IDriverLogic driverLogic, IHostLogic hostLogic, IUserLogic userLogic, IEmailServiceApi emailServiceApiApi, IApiEventService apiEventService)
    {
        _globalConfigs = globalConfigs;
        _studentLogic = studentLogic;
        _driverLogic = driverLogic;
        _hostLogic = hostLogic;
        _userLogic = userLogic;
        _emailServiceApiApi = emailServiceApiApi;
        _apiEventService = apiEventService;
    }

    public async Task<EmailFormViewModel> GetEmailForm()
    {
        var year = _globalConfigs.YearValue;
            
        return new EmailFormViewModel
        {
            AdminCount = (await _userLogic.GetAll()).Count(x => x.UserRoleEnum == UserRoleEnum.Admin),
            StudentCount = (await _studentLogic.GetAll(year)).Count(),
            DriverCount = (await _driverLogic.GetAll(year)).Count(),
            HostCount = (await _hostLogic.GetAll(year)).Count(),
            UserCount = (await _userLogic.GetAll()).Count()
        };
    }

    public async Task<bool> HandleEventEmail(EmailEventViewModel emailEventViewModel)
    {
        // Send the email
        await _emailServiceApiApi.SendEmailAsync(emailEventViewModel.Emails, emailEventViewModel.Subject, emailEventViewModel.Body);

        await _apiEventService.RecordEvent($"Sent form email [{emailEventViewModel.Subject}] to {string.Join(',', emailEventViewModel.Emails)}");

        return true;
    }
        
    /// <summary>
    /// Handles email form view model
    /// </summary>
    /// <param name="emailFormViewModel"></param>
    /// <returns></returns>
    public async Task<bool> HandleAdHocEmail(EmailFormViewModel emailFormViewModel)
    {
        var year = _globalConfigs.YearValue;
            
        var emailAddresses = new List<string>();

        // Add admin email
        if (emailFormViewModel.Admin)
        {
            var admins = (await _userLogic.GetAll()).Where(x => x.UserRoleEnum == UserRoleEnum.Admin)
                .Select(x => x.Email)
                .ToList();

            emailAddresses.AddRange(admins);
        }
            
        // Add student emails
        if (emailFormViewModel.Students)
        {
            var students = await _studentLogic.GetAll(year);
            emailAddresses.AddRange(students.Select(x => x.Email).Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        // Add driver emails
        if (emailFormViewModel.Drivers)
        {
            var drivers = await _driverLogic.GetAll(year);
            emailAddresses.AddRange(drivers.Select(x => x.Email).Where(x => !string.IsNullOrWhiteSpace(x)));
        }
            
        // Add host emails
        if (emailFormViewModel.Hosts)
        {
            var hosts = await _hostLogic.GetAll(year);
            emailAddresses.AddRange(hosts.Select(x => x.Email).Where(x => !string.IsNullOrWhiteSpace(x)));
        }
            
        // Add user emails
        if (emailFormViewModel.Users)
        {
            var users = await _userLogic.GetAll();
            emailAddresses.AddRange(users.Select(x => x.Email).Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        // Add additional emails
        if (!string.IsNullOrWhiteSpace(emailFormViewModel.AdditionalRecipients))
        {
            var emails = emailFormViewModel.AdditionalRecipients.Split(',').Select(x => x.Trim()).ToList();
            emailAddresses.AddRange(emails);
        }

        // Remove duplicates
        emailAddresses = emailAddresses.Distinct().ToList();

        // Send the email
        await _emailServiceApiApi.SendEmailAsync(emailAddresses, emailFormViewModel.Subject, emailFormViewModel.Message);

        await _apiEventService.RecordEvent($"Sent ad-hoc email [{emailFormViewModel.Subject}] to {string.Join(',', emailAddresses)}");

        return true;
    }

    /// <summary>
    /// Check-In all via an email
    /// </summary>
    /// <param name="entitiesEnum"></param>
    /// <param name="id"></param>
    /// <param name="present"></param>
    /// <returns></returns>
    public async Task<bool> HandleEmailCheckIn(EntitiesEnum entitiesEnum, int id, bool present)
    {
        switch (entitiesEnum)
        {
            case EntitiesEnum.Student:
                await _studentLogic.Update(id, student =>
                {
                    // Checked-in
                    student.IsPresent = present;
                });
                break;
            case EntitiesEnum.Driver:
                await _driverLogic.Update(id, driver =>
                {
                    // Checked-in
                    driver.IsPresent = present;
                });
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(entitiesEnum), entitiesEnum, null);
        }
            
        await _apiEventService.RecordEvent($"Handled [{entitiesEnum.GetName()}] check-in email response with ID: {id}");

        return true;
    }
}