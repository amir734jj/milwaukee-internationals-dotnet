using System.Threading.Tasks;
using API.Abstracts;
using API.Attributes;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Entities;
using Models.Enums;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Controllers.Api
{
    [UserRoleMiddleware(UserRoleEnum.Admin)]
    [AuthorizeMiddleware]
    [Route("api/[controller]")]
    public class EventController : BasicCrudController<Event>
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
        /// Returns instance of logic
        /// </summary>
        /// <returns></returns>
        public override IBasicCrudLogic<Event> BasicCrudLogic() => _eventLogic;

        /// <summary>
        /// Map Student to event
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("map/{eventId}/{studentId}")]
        [SwaggerOperation("MapStudentToEvent")]
        public async Task<IActionResult> MapStudent([FromRoute] int eventId, [FromRoute] int studentId)
        {
            var result = await _eventLogic.MapStudent(eventId, studentId);

            return Ok(result);
        }
        
        /// <summary>
        /// UnMap Student to event
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("unmap/{eventId}/{studentId}")]
        [SwaggerOperation("MapStudentToEvent")]
        public async Task<IActionResult> UnMapStudent([FromRoute] int eventId, [FromRoute] int studentId)
        {
            var result = await _eventLogic.UnMapStudent(eventId, studentId);

            return Ok(result);
        }
    }
}