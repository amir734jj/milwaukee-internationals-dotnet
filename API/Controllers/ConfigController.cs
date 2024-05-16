using System.Threading.Tasks;
using API.Attributes;
using DAL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Enums;

namespace API.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Route("[controller]")]
[AuthorizeMiddleware]
public class ConfigController : Controller
{
    private readonly IConfigLogic _configLogic;

    public ConfigController(IConfigLogic configLogic)
    {
        _configLogic = configLogic;
    }
        
    [HttpGet]
    [Route("")]
    public async Task<IActionResult> Index()
    {
        var result = await _configLogic.ResolveGlobalConfig();
            
        return View(result);
    }
        
    [HttpPost]
    [Route("")]
    [AuthorizeMiddleware(UserRoleEnum.Admin)]
    public async Task<IActionResult> UpdateConfig(GlobalConfigs globalConfigs)
    {
        await _configLogic.SetGlobalConfig(globalConfigs);

        return RedirectToAction("Index");
    }
}