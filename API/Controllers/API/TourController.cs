using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Constants;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers.API;

[AllowAnonymous]
[Route("api/[controller]")]
public class TourController : Controller
{
    private readonly GlobalConfigs _globalConfigs;

    public TourController(GlobalConfigs globalConfigs)
    {
        _globalConfigs = globalConfigs;
    }
        
    [HttpGet]
    [Route("info")]
    [SwaggerOperation("info")]
    public IActionResult Status()
    {
        return Ok(new
        {
            _globalConfigs.TourAddress,
            _globalConfigs.TourDate,
            _globalConfigs.TourLocation
        });
    }
}