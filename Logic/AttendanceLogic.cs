using Logic.Interfaces;
using Models.ViewModels;

namespace Logic
{
    public class AttendanceLogic : IAttendanceLogic
    {
        private readonly IStudentLogic _studentLogic;
        
        private readonly IDriverLogic _driverLogic;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="studentLogic"></param>
        /// <param name="driverLogic"></param>
        public AttendanceLogic(IStudentLogic studentLogic, IDriverLogic driverLogic)
        {
            _studentLogic = studentLogic;
            _driverLogic = driverLogic;
        }
        
        /// <summary>
        /// Set the attendance for student
        /// </summary>
        /// <param name="attendanceViewModel"></param>
        /// <returns></returns>
        public bool StudentSetAttendance(AttendanceViewModel attendanceViewModel)
        {
            var student = _studentLogic.Get(attendanceViewModel.Id);

            // Set attendance
            student.IsPressent = attendanceViewModel.Attendance;

            _studentLogic.Update(student.Id, student);

            return true;
        }

        /// <summary>
        /// Set the attendance for driver
        /// </summary>
        /// <param name="attendanceViewModel"></param>
        /// <returns></returns>
        public bool DriverSetAttendance(AttendanceViewModel attendanceViewModel)
        {
            var driver = _driverLogic.Get(attendanceViewModel.Id);

            // Set attendance
            driver.IsPressent = attendanceViewModel.Attendance;

            _driverLogic.Update(driver.Id, driver);

            return true;
        }
    }
}