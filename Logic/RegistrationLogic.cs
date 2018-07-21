using System;
using DAL.Interfaces;
using Logic.Interfaces;
using Models;

namespace Logic
{
    public class RegistrationLogic : IRegistrationLogic
    {
        private readonly IStudentLogic _studentLogic;
                
        private readonly IDriverLogic _driverLogic;
        
        private readonly IHostLogic _hostLogic;
        
        private readonly IEmailServiceApi _emailServiceApiApi;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="studentLogic"></param>
        /// <param name="driverLogic"></param>
        /// <param name="hostLogic"></param>
        /// <param name="emailServiceApiApi"></param>
        public RegistrationLogic(IStudentLogic studentLogic, IDriverLogic driverLogic, IHostLogic hostLogic, IEmailServiceApi emailServiceApiApi)
        {
            _studentLogic = studentLogic;
            _driverLogic = driverLogic;
            _hostLogic = hostLogic;
            _emailServiceApiApi = emailServiceApiApi;
        }
        
        /// <summary>
        /// Register driver
        /// </summary>
        /// <param name="driver"></param>
        /// <returns></returns>
        public bool RegisterDriver(Driver driver)
        {
            var result = _driverLogic.Save(driver);

            // If save was successful
            if (result != null)
            {
                _emailServiceApiApi.SendEmailAsync(driver.Email, "Tour of Milwaukee: Driver registration", $@"
                    <p> This is an automatically generated email. </p>
                    <p> ----------------------------------------- </p>
                    <p> Name: {driver.Fullname}</p>
                    <p> Role: {driver.Role}</p>
                    <p> Phone: {driver.Phone}</p>
                    <p> Capacity: {driver.Capacity}</p>
                    <p> Require Navigator: {(driver.RequireNavigator ? "Yes" : $"No, navigator: {driver.Navigator}")}</p>
                    <br>                                                                                         
                    <p> 2018 Tour of Milwaukee</p>
                    <p> Date: August 25, 2018</p> 
                    <p> Time: 12:30 pm (Brief orientation only for drivers and navigators) </p> 
                    <p> Address: 2200 E Kenwood Blvd, Milwaukee, WI 53211 </p> 
                    <p> Location: Union Ballroom</p>
                    <br>                                                                      
                    <p> Thank you for helping with the tour this year. Reply to this email will be sent automatically to the team.</p>
                    <p> For questions, comments and feedback, please contact Asher Imtiaz (414-499-5360) or Marie Wilke (414-852-5132).</p>
                    <br>                                                                                                                   
                    <p> Blessings, </p> 
                ");
            }

            return true;
        }

        /// <summary>
        /// Registers student
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        public bool RegisterStudent(Student student)
        {
            var result = _studentLogic.Save(student);

            // If save was successful
            if (result != null)
            {
                _emailServiceApiApi.SendEmailAsync(student.Email, "Tour of Milwaukee: Driver registration", $@"
                    <p>Name: {student.Fullname}</p>       
                    <p>Email: {student.Email}</p>
                    <p>University: {student.University}</p>          
                    <p>Major: {student.Major}</p>          
                    <p>Phone: {student.Phone}</p>          
                    <hr>                                
                    <p>See you at the Tour of Milwaukee</p> 
                    <br>                                    
                    <p> 2017 Tour of Milwaukee Registration</p> 
                    <p> Date: August 26, 2018</p> 
                    <p> Time: 12:00 noon</p> 
                    <p> Address: 2200 E Kenwood Blvd, Milwaukee, WI 53211 </p> 
                    <p> Location: Union Ballroom</p> 
                    <p> Thank you for registering for this event. Please share this with other new international friends.</p> 
                    <p> If you need any sort of help (furniture, moving, etc.), please contact Asher Imtiaz (414-499-5360) or Amanda Johnson (414-801-4431) on campus.</p> 
                ");
            }

            return true;
        }

        /// <summary>
        /// Registers host
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public bool RegisterHost(Host host)
        {
            var result = _hostLogic.Save(host);

            return true;
        }
    }
}
