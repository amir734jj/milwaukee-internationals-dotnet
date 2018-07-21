using System.Collections.Generic;

namespace Models.ViewModels
{
    /// <summary>
    /// Student-Driver mapping all
    /// </summary>
    public class StudentDriverMappingViewModel
    {
        public IEnumerable<Driver> AvailableDrivers { get; set; }
        
        public IEnumerable<Student> AvailableStudents { get; set; }
        
        public IEnumerable<Driver> MappedDrivers { get; set; }
        
        public IEnumerable<Student> MappedStudents { get; set; }
    }
}