using System.Collections.Generic;
using System.Linq;
using Logic.Interfaces;
using Models;
using Models.Interfaces;
using Models.ViewModels;

namespace Logic
{
    public class StudentDriverMappingLogic : IStudentDriverMappingLogic
    {
        private readonly IStudentLogic _studentLogic;
        
        private readonly IDriverLogic _driverLogic;

        /// <summary>
        /// Student-Driver mapping logic
        /// </summary>
        /// <param name="studentLogic"></param>
        /// <param name="driverLogic"></param>
        public StudentDriverMappingLogic(IStudentLogic studentLogic, IDriverLogic driverLogic)
        {
            _studentLogic = studentLogic;
            _driverLogic = driverLogic;
        }

        /// <summary>
        /// Logic to handle the mapping
        /// </summary>
        /// <param name="newStudentDriverMappingViewModel"></param>
        /// <returns></returns>
        public bool MapStudentToDriver(NewStudentDriverMappingViewModel newStudentDriverMappingViewModel)
        {
            var driver = _driverLogic.Get(newStudentDriverMappingViewModel.DriverId);
            var student = _studentLogic.Get(newStudentDriverMappingViewModel.StudentId);

            // Initialize the list if it not already initialized
            driver.Students = driver.Students ?? new List<Student>();
            
            // Add the map
            student.Driver = driver;

            // Save changes
            _studentLogic.Update(student.Id, student);

            return true;
        }

        /// <summary>
        /// Un-Map student from driver
        /// </summary>
        /// <param name="newStudentDriverMappingViewModel"></param>
        /// <returns></returns>
        public bool UnMapStudentToDriver(NewStudentDriverMappingViewModel newStudentDriverMappingViewModel)
        {
            var driver = _driverLogic.Get(newStudentDriverMappingViewModel.DriverId);
            var student = _studentLogic.Get(newStudentDriverMappingViewModel.StudentId);

            // Initialize the list if it not already initialized
            student.Driver = null;

            // Save changes
            _studentLogic.Update(student.Id, student);
            
            return true;
        }

        /// <summary>
        /// Returns the status of mappings
        /// </summary>
        /// <returns></returns>
        public StudentDriverMappingViewModel MappingStatus()
        {
            var students = _studentLogic.GetAll().ToList();
            var drivers = _driverLogic.GetAll().ToList();
            
            // TODO: add check to return only students that are pressent
            return new StudentDriverMappingViewModel
            {
                AvailableStudents = students.Where(x => x.Driver == null & x.IsPressent),
                AvailableDrivers = drivers.Where(x => x.Capacity >= x.Students?.Count),
                MappedDrivers = drivers.Where(x => x.Students != null && x.Students.Any()),
                MappedStudents = students.Where(x => x.Driver != null)
            };
        }
    }
}