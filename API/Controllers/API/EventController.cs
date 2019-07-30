using System.Threading.Tasks;
using API.Abstracts;
using API.Attributes;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Entities;
using Models.Enums;
using Models.ViewModels;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Controllers.Api
{
    [UserRoleMiddleware(UserRoleEnum.Admin)]
    [AuthorizeMiddleware]
    [Route("api/[controller]")]
    public class EventController : BasicCrudController<Event>
    {
        private readonly IEventLogic _eventLogic;
        
        private readonly IEmailUtilityLogic _emailUtilityLogic;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="eventLogic"></param>
        /// <param name="emailUtilityLogic"></param>
        public EventController(IEventLogic eventLogic, IEmailUtilityLogic emailUtilityLogic)
        {
            _eventLogic = eventLogic;
            _emailUtilityLogic = emailUtilityLogic;
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
        
        
        /// <summary>
        /// Registers an event
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Info/{id}")]
        public async Task<IActionResult> Info([FromRoute] int id)
        {
            var eventInfo = await _eventLogic.GetEventInfo(id);

            return Ok(eventInfo);
        }
        
        /// <summary>
        /// Event email
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Email")]
        public async Task<IActionResult> EventEmail(EmailEventViewModel emailEventViewModel)
        {
            var rslt = await _emailUtilityLogic.HandleEventEmail(emailEventViewModel);

            return Ok(rslt);
        }
    }
}