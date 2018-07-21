using API.Attributes;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModels;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Controllers.Api
{
    [AuthorizeMiddleware]
    [Route("api/[controller]")]
    public class AttendanceController : Controller
    {
        private readonly IAttendanceLogic _attendanceLogic;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="attendanceLogic"></param>
        public AttendanceController(IAttendanceLogic attendanceLogic)
        {
            _attendanceLogic = attendanceLogic;
        }

        /// <summary>
        /// Returns the status of mappings
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("SetAttendance")]
        [SwaggerOperation("SetAttendance")]
        public IActionResult SetAttendance([FromBody] AttendanceViewModel attendanceViewModel)
        {
            return Ok(_attendanceLogic.SetAttendance(attendanceViewModel));
        }
    }
}