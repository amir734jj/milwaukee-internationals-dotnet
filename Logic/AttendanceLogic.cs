using System.Linq;
using System.Threading.Tasks;
using DAL.Extensions;
using DAL.Interfaces;
using Logic.Interfaces;
using Models.Constants;
using Models.ViewModels;

namespace Logic
{
    public class AttendanceLogic : IAttendanceLogic
    {
        private readonly IStudentLogic _studentLogic;
        
        private readonly IDriverLogic _driverLogic;
        
        private readonly IEmailServiceApi _emailServiceApi;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="studentLogic"></param>
        /// <param name="driverLogic"></param>
        /// <param name="emailServiceApi"></param>
        public AttendanceLogic(IStudentLogic studentLogic, IDriverLogic driverLogic, IEmailServiceApi emailServiceApi)
        {
            _studentLogic = studentLogic;
            _driverLogic = driverLogic;
            _emailServiceApi = emailServiceApi;
        }
        
        /// <summary>
        /// Set the attendance for student
        /// </summary>
        /// <param name="attendanceViewModel"></param>
        /// <returns></returns>
        public async Task<bool> StudentSetAttendance(AttendanceViewModel attendanceViewModel)
        {
            await _studentLogic.Update(attendanceViewModel.Id, x =>
            {
                // Set attendance
                x.IsPressent = attendanceViewModel.Attendance;
            });

            return true;
        }

        /// <summary>
        /// Set the attendance for driver
        /// </summary>
        /// <param name="attendanceViewModel"></param>
        /// <returns></returns>
        public async Task<bool> DriverSetAttendance(AttendanceViewModel attendanceViewModel)
        {
            // Set attendance
            await _driverLogic.Update(attendanceViewModel.Id, x =>
            {
                // Set attendance
                x.IsPressent = attendanceViewModel.Attendance;
            });

            return true;
        }

        /// <summary>
        /// Handles sending email to students so they check-in
        /// </summary>
        /// <returns></returns>
        public async Task<bool> HandleStudentSendCheckIn()
        {
            (await _studentLogic.GetAll()).ForEach(x =>
            {
                var url = $"{ApiConstants.WebSiteApiUrl}/utility/EmailCheckIn/Student/{x.GetHashCode()}";
                
                _emailServiceApi.SendEmailAsync(x.Email, "Tour Check-In", $@"
                    <h4>Please use this link to check-in</h4>
                    <br>
                    <p><a href=""{url}"">Link</a> ({url})</p>
                    <br>
                    <p>Thank you</p>
                ");
            });
            
            return true;
        }

        /// <summary>
        /// Handles sending email to drivers so they check-in
        /// </summary>
        /// <returns></returns>
        public async Task<bool> HandleDriverSendCheckIn()
        {
            (await _driverLogic.GetAll()).ForEach(x =>
            {
                var url = $"{ApiConstants.WebSiteUrl}/utility/EmailCheckIn/Driver/{x.GetHashCode()}";
                
                _emailServiceApi.SendEmailAsync("amirhesamyan@gmail.com", "Tour Driver Check-In", $@"
                    <h4>Hello {x.Fullname}</h4>
                    <h4>Please use this link to check-in</h4>
                    <p><a href=""{url}"">Link</a> ({url})</p>
                    <br>
                    <p>Thank you</p>
                ");
            });

            return true;
        }
    }
}