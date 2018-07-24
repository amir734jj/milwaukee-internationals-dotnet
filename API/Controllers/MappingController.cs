using API.Attributes;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [AuthorizeMiddleware]
    [Route("[controller]")]
    public class MappingController : Controller
    {
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
        [SwaggerOperation("StudentDriverMapping")]
        public IActionResult StudentDriverMapping()
        {
            return View();
        }

        /// <summary>
        /// Returns Driver-Host Mapping view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("DriverHostMapping")]
        [SwaggerOperation("DriverHostMapping")]
        public IActionResult DriverHostMapping()
        {
            return View();
        }
    }
}