using System.Threading.Tasks;
using API.Attributes;
using Microsoft.AspNetCore.Mvc;
using Models.Constants;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Controllers.API
{
    [AuthorizeMiddleware]
    [Route("api/[controller]")]
    public class ConfigController : Controller
    {
        [HttpGet]
        [Route("Status")]
        [SwaggerOperation("Status")]
        public async Task<IActionResult> Status()
        {
            var config = GlobalConfigs.ToAnonymousObject();
            
            return Ok(config);
        }
    }
}