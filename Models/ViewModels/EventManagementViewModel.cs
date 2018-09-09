using System.Collections.Generic;

namespace Models.ViewModels
{
    public class EventManagementViewModel
    {
        public Event Event { get; set; }
        
        public IEnumerable<Student> AvailableStudents { get; set; }
    }
}