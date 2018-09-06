using System.Threading.Tasks;
using API.Attributes;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [AuthorizeMiddleware]
    [Route("[controller]")]
    public class EventController : Controller
    {
        private readonly IEventLogic _eventLogic;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="eventLogic"></param>
        public EventController(IEventLogic eventLogic)
        {
            _eventLogic = eventLogic;
        }
        
        /// <summary>
        /// Returns driver view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [SwaggerOperation("Index")]
        public async Task<IActionResult> Index()
        {
            var events = await _eventLogic.GetAll();
            
            return View(events);
        }
        
        /// <summary>
        /// Returns driver view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Delete/{id}")]
        [SwaggerOperation("Delete")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var events = await _eventLogic.Delete(id);

            return RedirectToAction("Index");
        }
    }
}