using System;
using System.Collections.Generic;
using System.Linq;
using DAL.Extensions;
using DAL.Interfaces;
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
        
        private readonly IEmailServiceApi _emailServiceApi;

        /// <summary>
        /// Student-Driver mapping logic
        /// </summary>
        /// <param name="studentLogic"></param>
        /// <param name="driverLogic"></param>
        /// <param name="emailServiceApi"></param>
        public StudentDriverMappingLogic(IStudentLogic studentLogic, IDriverLogic driverLogic, IEmailServiceApi emailServiceApi)
        {
            _studentLogic = studentLogic;
            _driverLogic = driverLogic;
            _emailServiceApi = emailServiceApi;
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
            
            // Add the map to student
            student.Driver = driver;

            // Save changes to student
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

            // Remove the map from student
            student.Driver = null;

            // Save changes to student
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
                AvailableDrivers = drivers.Where(x => x.Capacity >= (x.Students ?? new List<Student>()).Count),
                MappedDrivers = drivers.Where(x => x.Students != null && x.Students.Any()),
                MappedStudents = students.Where(x => x.Driver != null)
            };
        }
        
        /// <summary>
        /// Emails the mappings to drivers
        /// </summary>
        /// <returns></returns>
        public bool EmailMappings()
        {
            string MessageFunc(Driver driver)
            {
                return $@"
        <p> **This is an automatically generated email** </p>                      
        <br>                                                                    
        <p> Hello {driver.Fullname},</p>
        <p> Your Driver ID:<strong> {driver.DisplayId} </strong></p> 
        <p> Students: </p>                       
        <ul>                                                                    
            {string.Join(Environment.NewLine, driver.Students?.Select(student => $"<li>{student.Fullname} ({student.Country})</li>") ?? new List<string>())}                                                    
        </ul>                                                                   
        <br>
            {(driver.Host != null ? $@"
          <p> Host Name: {driver.Host.Fullname} </p>                              
          <p> Host Contact: {driver.Host.Phone} </p>                              
          <p> Host Address: {driver.Host.Address} </p>                            
            " : string.Empty)}
        <br>                                                                    
        <br>                                                                    
        <p> Thank you for helping with the tour this year. Reply to this email will be sent automatically to the team.</p>      
        <p> For questions, comments and feedback, please contact Asher Imtiaz (414-499-5360) or Marie Wilke (414-852-5132).</p> 
        ";
            }

            // Send the email to drivers
            _driverLogic.GetAll().ForEach(x => _emailServiceApi.SendEmailAsync(x.Email, "Tour of Milwaukee - Assigned Students", MessageFunc(x)));

            // Return true
            return true;
        }
    }
}