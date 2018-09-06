using API.Abstracts;
using API.Attributes;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace API.Controllers.Api
{
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
    }
}