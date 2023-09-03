using System.Linq;
using System.Threading.Tasks;
using API.Attributes;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[AuthorizeMiddleware]
[Route("[controller]")]
public class MappingController : Controller
{
    private readonly IStudentLogic _studentLogic;
    private readonly IDriverLogic _driverLogic;
    private readonly ISmsUtilityLogic _smsUtilityLogic;

    public MappingController(IStudentLogic studentLogic, IDriverLogic driverLogic, ISmsUtilityLogic smsUtilityLogic)
    {
        _studentLogic = studentLogic;
        _driverLogic = driverLogic;
        _smsUtilityLogic = smsUtilityLogic;
    }
        
    // GET the view
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Returns Student-Driver Mapping view
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("StudentDriverMapping")]
    public async Task<IActionResult> StudentDriverMapping()
    {
        var students = (await _studentLogic.GetAll()).ToList();
            
        return View(students);
    }

    /// <summary>
    /// Returns Driver-Host Mapping view
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("DriverHostMapping")]
    public async Task<IActionResult> DriverHostMapping()
    {
        var drivers = (await _driverLogic.GetAll()).ToList();

        return View(drivers);
    }

    /// <summary>
    /// Host heads up SMS
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("HostHeadsUpSms")]
    public async Task<IActionResult> HostHeadsUpSms()
    {
        await _smsUtilityLogic.HandleHostSms();

        return RedirectToAction("DriverHostMapping");
    }
}
