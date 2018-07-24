using System.Collections.Generic;
using Models;
using Models.ViewModels;

namespace Logic.Interfaces
{
    public interface IAttendanceLogic
    {
        bool StudentSetAttendance(AttendanceViewModel attendanceViewModel);
        
        bool DriverSetAttendance(AttendanceViewModel attendanceViewModel);
    }
}