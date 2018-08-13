using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class UtilityController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return
            View();
        }
    }
}