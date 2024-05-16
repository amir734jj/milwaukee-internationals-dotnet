using System.Threading.Tasks;
using DAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers.API;

[AllowAnonymous]
[Route("api/[controller]")]
public class TourController : Controller
{
    private readonly IConfigLogic _configLogic;

    public TourController(IConfigLogic configLogic)
    {
        _configLogic = configLogic;
    }
        
    [HttpGet]
    [Route("info")]
    [SwaggerOperation("info")]
    public async Task<IActionResult> Status()
    {
        var globalConfigs = await _configLogic.ResolveGlobalConfig();
        
        return Ok(new
        {
            globalConfigs.TourAddress,
            globalConfigs.TourDate,
            globalConfigs.TourLocation
        });
    }
}