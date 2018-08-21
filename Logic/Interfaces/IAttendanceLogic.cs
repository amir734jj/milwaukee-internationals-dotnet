using System.Collections.Generic;
using System.Threading.Tasks;
using Models;
using Models.ViewModels;

namespace Logic.Interfaces
{
    public interface IAttendanceLogic
    {
        Task<bool> StudentSetAttendance(AttendanceViewModel attendanceViewModel);
        
        Task<bool> DriverSetAttendance(AttendanceViewModel attendanceViewModel);

        Task<bool> HandleStudentSendCheckIn();

        Task<bool> HandleDriverSendCheckIn();
    }
}