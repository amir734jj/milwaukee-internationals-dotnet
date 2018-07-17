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
                _emailServiceApiApi.SendEmailAsync(driver.Email, "Driver registration", "Successful!");
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