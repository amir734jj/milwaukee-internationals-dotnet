using API.Attributes;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [AuthorizeMiddleware]
    [Route("[controller]")]
    public class DriverController : Controller
    {
        private readonly IDriverLogic _driverLogic;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="driverLogic"></param>
        public DriverController(IDriverLogic driverLogic)
        {
            _driverLogic = driverLogic;
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
            return View(_driverLogic.GetAll());
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
            _driverLogic.Delete(id);

            return RedirectToAction("Index");
        }
    }
}