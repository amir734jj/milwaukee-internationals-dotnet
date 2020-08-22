using System.Threading.Tasks;
using API.Attributes;
using Microsoft.AspNetCore.Mvc;
using Models.Constants;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers.API
{
    [AuthorizeMiddleware]
    [Route("api/[controller]")]
    public class ConfigController : Controller
    {
        private readonly GlobalConfigs _globalConfigs;

        public ConfigController(GlobalConfigs globalConfigs)
        {
            _globalConfigs = globalConfigs;
        }
        
        [HttpGet]
        [Route("Status")]
        [SwaggerOperation("Status")]
        public async Task<IActionResult> Status()
        {
            var config = _globalConfigs.ToAnonymousObject();
            
            return Ok(config);
        }
    }
}