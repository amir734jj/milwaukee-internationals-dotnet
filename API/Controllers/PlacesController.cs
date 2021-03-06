using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    [Route("[controller]")]
    public class PlacesController : Controller
    {
        [HttpGet]
        [Route("{year}")]
        public IActionResult Place(int year)
        {
            return View();
        }
    }
}