using API.Attributes;
using Microsoft.AspNetCore.Mvc;
using Models.Interfaces;
using Models.ViewModels;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Controllers.Api
{
    [AuthorizeMiddleware]
    [Route("api/[controller]")]
    public class StudentDriverMappingController : Controller
    {
        private readonly IStudentDriverMappingLogic _studentDriverMappingLogic;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="studentDriverMappingLogic"></param>
        public StudentDriverMappingController(IStudentDriverMappingLogic studentDriverMappingLogic)
        {
            _studentDriverMappingLogic = studentDriverMappingLogic;
        }
        
        /// <summary>
        /// Returns the status of mappings
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Status")]
        [SwaggerOperation("StudentDriverMappingStatus")]
        public IActionResult StudentDriverMappingStatus()
        {
            return Ok(_studentDriverMappingLogic.MappingStatus());
        }
        
        /// <summary>
        /// Maps the student to driver
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Map")]
        [SwaggerOperation("StudentDriverMappingMap")]
        public IActionResult StudentDriverMappingMap([FromBody] NewStudentDriverMappingViewModel newStudentDriverMappingViewModel)
        {
            return Ok(_studentDriverMappingLogic.MapStudentToDriver(newStudentDriverMappingViewModel));
        }
        
        /// <summary>
        /// Un-Maps student from driver
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("UnMap")]
        [SwaggerOperation("StudentDriverMappingUnMap")]
        public IActionResult StudentDriverMappingUnMap([FromBody] NewStudentDriverMappingViewModel newStudentDriverMappingViewModel)
        {
            return Ok(_studentDriverMappingLogic.UnMapStudentToDriver(newStudentDriverMappingViewModel));
        }
    }
}