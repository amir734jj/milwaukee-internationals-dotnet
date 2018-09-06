using System.Threading.Tasks;
using API.Attributes;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Enums;
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
        [UserRoleMiddleware(UserRoleEnum.Admin)]
        [HttpGet]
        [Route("Delete/{id}")]
        [SwaggerOperation("Delete")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await _eventLogic.Delete(id);

            return RedirectToAction("Index");
        }
        
        /// <summary>
        /// Registers an event
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Register")]
        [SwaggerOperation("Register")]
        public async Task<IActionResult> RegisterEvent(Event @event)
        {
            await _eventLogic.Save(@event);

            return RedirectToAction("Index");
        }
        
        /// <summary>
        /// Registers an event
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Info/{id}")]
        [SwaggerOperation("Register")]
        public async Task<IActionResult> EventInfo([FromRoute] int id)
        {
            var @event = await _eventLogic.Get(id);

            return View(@event);
        }
    }
}