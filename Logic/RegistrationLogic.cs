using System;
using DAL.Interfaces;
using Logic.Interfaces;
using Models;

namespace Logic
{
    public class RegistrationLogic : IRegistrationLogic
    {
        private readonly IEmailServiceApi _emailServiceApiApi;
        
        private readonly IDriverLogic _driverLogic;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="driverLogic"></param>
        /// <param name="emailServiceApiApi"></param>
        public RegistrationLogic(IDriverLogic driverLogic, IEmailServiceApi emailServiceApiApi)
        {
            _driverLogic = driverLogic;
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
                    <p> Role: {driver.Role}></p>                                                                                                                                                     
                    <br>                                                                                                                   
                    <p> 2018 Tour of Milwaukee</p> 
                    <p> Date: August 26, 2018</p> 
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
        /// Registes student
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool RegisterStudent(Student student)
        {
            throw new NotImplementedException();
        }
    }
}