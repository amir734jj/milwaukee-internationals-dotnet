using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [AllowAnonymous]
    [Route("[controller]")]
    public class ErrorController : Controller
    {
        [Route("{statusCode?}")]
        [HttpGet]
        public IActionResult Index(int statusCode = 400)
        {
            return View(statusCode);
        }
    }
}