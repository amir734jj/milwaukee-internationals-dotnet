using System.Collections.Generic;
using Models;
using Models.ViewModels;

namespace Logic.Interfaces
{
    public interface IAttendanceLogic
    {
        bool SetAttendance(AttendanceViewModel attendanceViewModel);
    }
}