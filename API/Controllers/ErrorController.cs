using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [AllowAnonymous]
    [Route("[controller]")]
    public class ErrorController : Controller
    {
        [Route("")]
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Something went wrong!");
        }
    }
}