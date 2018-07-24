using API.Attributes;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [AuthorizeMiddleware]
    [Route("[controller]")]
    public class HostController : Controller
    {
        private readonly IHostLogic _hostLogic;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="hostLogic"></param>
        public HostController(IHostLogic hostLogic)
        {
            _hostLogic = hostLogic;
        }
        
        /// <summary>
        /// Returns driver view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [SwaggerOperation("Index")]
        public IActionResult Index()
        {
            return View(_hostLogic.GetAll());
        }
        
        /// <summary>
        /// Delete a driver
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Delete/{id}")]
        [SwaggerOperation("Delete")]
        public IActionResult Delete(int id)
        {
            _hostLogic.Delete(id);

            return RedirectToAction("Index");
        }
    }
}