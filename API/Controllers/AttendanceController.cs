using System.Threading.Tasks;
using API.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [AuthorizeMiddleware]
    [Route("[controller]")]
    public class AttendanceController : Controller
    {
        // GET
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }
        
        // GET
        [HttpGet]
        [Route("Student")]
        public async Task<IActionResult> Student()
        {
            return View();
        }
        
        // GET
        [HttpGet]
        [Route("Driver")]
        public async Task<IActionResult> Driver()
        {
            return View();
        }
    }
}