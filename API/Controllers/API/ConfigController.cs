using System.Threading.Tasks;
using API.Attributes;
using DAL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers.API;

[AuthorizeMiddleware]
[Route("api/[controller]")]
public class ConfigController : Controller
{
    private readonly IConfigLogic _configLogic;

    public ConfigController(IConfigLogic configLogic)
    {
        _configLogic = configLogic;
    }
        
    [HttpGet]
    [Route("")]
    [SwaggerOperation("Status")]
    public async Task<IActionResult> Status()
    {
        return Ok(await _configLogic.ResolveGlobalConfig());
    }
}