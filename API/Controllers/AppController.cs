using API.Attributes;
using Microsoft.AspNetCore.Mvc;
using Models.Enums;

namespace API.Controllers
{
    /// <summary>
    ///     App controller
    /// </summary>
    [AuthorizeMiddleware]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("[controller]")]
    public class AppController : Controller
    {
        [HttpGet]
        [Route("CheckIn/Student/{hashcode:int}")]
        public IActionResult StudentCheckIn([FromRoute] int hashcode)
        {
            return RedirectToAction("EmailCheckIn", "Utility", new
            {
                type = EntitiesEnum.Student, hashcode
            });
        }
    }
}