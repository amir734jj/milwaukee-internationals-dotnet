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
        [Route("Student/SetAttendance")]
        [SwaggerOperation("SetAttendance")]
        public IActionResult StudentSetAttendance([FromBody] AttendanceViewModel attendanceViewModel)
        {
            return Ok(_attendanceLogic.StudentSetAttendance(attendanceViewModel));
        }
        
        /// <summary>
        /// Returns the status of mappings
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Driver/SetAttendance")]
        [SwaggerOperation("SetAttendance")]
        public IActionResult DriverSetAttendance([FromBody] AttendanceViewModel attendanceViewModel)
        {
            return Ok(_attendanceLogic.DriverSetAttendance(attendanceViewModel));
        }
        
        /// <summary>
        /// Send check-in email to drivers
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Driver/SendCheckIn")]
        [SwaggerOperation("DriverSendCheckIn")]
        public IActionResult DriverSendCheckIn()
        {
            return Ok(_attendanceLogic.HandleDriverSendCheckIn());
        }
        
        /// <summary>
        /// Send check-in email to students
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Student/SendCheckIn")]
        [SwaggerOperation("StudentSendCheckIn")]
        public IActionResult StudentSendCheckIn()
        {
            return Ok(_attendanceLogic.HandleStudentSendCheckIn());
        }
    }
}