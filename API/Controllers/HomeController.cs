using System.Threading.Tasks;
using API.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        private readonly HttpRequestUtilityBuilder _httpRequestUtilityBuilder;

        public HomeController(HttpRequestUtilityBuilder httpRequestUtilityBuilder)
        {
            _httpRequestUtilityBuilder = httpRequestUtilityBuilder;
        }
        
        public async Task<IActionResult> Index()
        {
            return Redirect("~/Registration/Student".ToLower());
        }

        /// <summary>
        /// View page to register
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Register")]
        public async Task<IActionResult> Register()
        {
            return Redirect("~/Identity/Register".ToLower());
        }
        
        /// <summary>
        /// View page to register
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Echo")]
        public async Task<IActionResult> Echo()
        {
            return Ok(await _httpRequestUtilityBuilder.For(HttpContext).GetUserInfo());
        }
    }
}