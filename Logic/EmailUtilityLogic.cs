using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Interfaces;
using Logic.Interfaces;
using Models.Enums;
using Models.ViewModels;

namespace Logic
{
    public class EmailUtilityLogic :  IEmailUtilityLogic
    {
        private readonly IUserLogic _userLogic;
        private readonly IEmailServiceApi _emailServiceApiApi;
        private readonly IStudentLogic _studentLogic;
        private readonly IHostLogic _hostLogic;
        private readonly IDriverLogic _driverLogic;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="studentLogic"></param>
        /// <param name="driverLogic"></param>
        /// <param name="hostLogic"></param>
        /// <param name="userLogic"></param>
        /// <param name="emailServiceApiApi"></param>
        public EmailUtilityLogic(IStudentLogic studentLogic, IDriverLogic driverLogic, IHostLogic hostLogic, IUserLogic userLogic, IEmailServiceApi emailServiceApiApi)
        {
            _studentLogic = studentLogic;
            _driverLogic = driverLogic;
            _hostLogic = hostLogic;
            _userLogic = userLogic;
            _emailServiceApiApi = emailServiceApiApi;
        } 
        
        /// <summary>
        /// Handles email form view model
        /// </summary>
        /// <param name="emailFormViewModel"></param>
        /// <returns></returns>
        public async Task<bool> HandleAdHocEmail(EmailFormViewModel emailFormViewModel)
        {
            var emailAddresses = new List<string>();

            // Add student emails
            if (emailFormViewModel.Students)
            {
                emailAddresses.AddRange(_studentLogic.GetAll().Result.Select(x => x.Email).Where(x => !string.IsNullOrWhiteSpace(x)));
            }

            // Add driver emails
            if (emailFormViewModel.Drivers)
            {
                emailAddresses.AddRange(_driverLogic.GetAll().Result.Select(x => x.Email).Where(x => !string.IsNullOrWhiteSpace(x)));
            }
            
            // Add host emails
            if (emailFormViewModel.Hosts)
            {
                emailAddresses.AddRange(_hostLogic.GetAll().Result.Select(x => x.Email).Where(x => !string.IsNullOrWhiteSpace(x)));
            }
            
            // Add user emails
            if (emailFormViewModel.Users)
            {
                emailAddresses.AddRange(_userLogic.GetAll().Result.Select(x => x.Email).Where(x => !string.IsNullOrWhiteSpace(x)));
            }

            // Remove duplicates
            emailAddresses = emailAddresses.Distinct().ToList();

            // Send the eamil
            await _emailServiceApiApi.SendEmailAsync(emailAddresses, emailFormViewModel.Subject, emailFormViewModel.Message);

            return true;
        }

        /// <summary>
        /// Check-In all via an email
        /// </summary>
        /// <param name="entitiesEnum"></param>
        /// <param name="id"></param>
        /// <param name="present"></param>
        /// <returns></returns>
        public bool HandleEmailCheckIn(EntitiesEnum entitiesEnum, int id, bool present)
        {
            switch (entitiesEnum)
            {
                case EntitiesEnum.Student:
                    _studentLogic.Update(id, student =>
                    {
                        // Checked-in
                        student.IsPressent = present;
                    });
                    break;
                case EntitiesEnum.Driver:
                    _driverLogic.Update(id, driver =>
                    {
                        // Checked-in
                        driver.IsPressent = present;
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(entitiesEnum), entitiesEnum, null);
            }

            return true;
        }
    }
}