using System.Threading.Tasks;
using API.Attributes;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Enums;

namespace API.Controllers;

[AuthorizeMiddleware(UserRoleEnum.Admin)]
[Route("[controller]")]
public class SmsController : Controller
{
    private readonly ISmsUtilityLogic _smsUtilityLogic;

    public SmsController(ISmsUtilityLogic smsUtilityLogic)
    {
        _smsUtilityLogic = smsUtilityLogic;
    }

    [HttpGet]
    [Route("Driver")]
    public async Task<ActionResult> SendDriverSms()
    {
        await _smsUtilityLogic.HandleDriverSms();

        return RedirectToAction("Driver", "Attendance");
    }

    [HttpGet]
    [Route("Student")]
    public async Task<ActionResult> SendStudentSms()
    {
        await _smsUtilityLogic.HandleStudentSms();

        return RedirectToAction("Student", "Attendance");
    }
}