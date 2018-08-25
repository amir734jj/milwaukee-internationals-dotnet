using System.Linq;
using System.Threading.Tasks;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        private IDriverLogic _driverLogic;

        public HomeController(IDriverLogic driverLogic)
        {
            _driverLogic = driverLogic;
        }
        
        public async Task<IActionResult> Index()
        {
            return Ok(_driverLogic.GetAll().Result.Select(x => x.GetHashCode()));
        }

        /// <summary>
        /// View page to register
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Register")]
        [SwaggerOperation("Register")]
        public async Task<IActionResult> Register()
        {
            return Redirect("~/Identity/Register".ToLower());
        }
    }
}