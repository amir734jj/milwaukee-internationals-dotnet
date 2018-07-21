using Logic.Interfaces;
using Models.ViewModels;

namespace Logic
{
    public class AttendanceLogic : IAttendanceLogic
    {
        private readonly IStudentLogic _studentLogic;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="studentLogic"></param>
        public AttendanceLogic(IStudentLogic studentLogic)
        {
            _studentLogic = studentLogic;
        }
        
        /// <summary>
        /// Set the attendance
        /// </summary>
        /// <param name="attendanceViewModel"></param>
        /// <returns></returns>
        public bool SetAttendance(AttendanceViewModel attendanceViewModel)
        {
            var student = _studentLogic.Get(attendanceViewModel.StudentId);

            // Set attendance
            student.IsPressent = attendanceViewModel.Attendance;

            _studentLogic.Update(student.Id, student);

            return true;
        }
    }
}