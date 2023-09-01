using System.Threading.Tasks;
using API.Attributes;
using Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Enums;

namespace API.Controllers;

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
    [AuthorizeMiddleware(UserRoleEnum.Admin)]
    public async Task<ActionResult> SendDriverSms()
    {
        await _smsUtilityLogic.HandleDriverSms();

        return RedirectToAction("Driver", "Attendance");
    }

    [HttpGet]
    [Route("Incoming")]
    [AllowAnonymous]
    public async Task<ActionResult> IncomingSms([FromBody]object body)
    {
        await _smsUtilityLogic.IncomingSms(body);

        return Ok("received");
    }
}