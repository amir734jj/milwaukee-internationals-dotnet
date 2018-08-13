using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Interfaces;
using Logic.Interfaces;
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
        public async Task<bool> Handle(EmailFormViewModel emailFormViewModel)
        {
            var emailAddresses = new List<string>();

            // Add student emails
            if (emailFormViewModel.Students)
            {
                emailAddresses.AddRange(_studentLogic.GetAll().Select(x => x.Email).Where(x => !string.IsNullOrWhiteSpace(x)));
            }

            // Add driver emails
            if (emailFormViewModel.Drivers)
            {
                emailAddresses.AddRange(_driverLogic.GetAll().Select(x => x.Email).Where(x => !string.IsNullOrWhiteSpace(x)));
            }
            
            // Add host emails
            if (emailFormViewModel.Hosts)
            {
                emailAddresses.AddRange(_hostLogic.GetAll().Select(x => x.Email).Where(x => !string.IsNullOrWhiteSpace(x)));
            }
            
            // Add user emails
            if (emailFormViewModel.Users)
            {
                emailAddresses.AddRange(_userLogic.GetAll().Select(x => x.Email).Where(x => !string.IsNullOrWhiteSpace(x)));
            }

            // Remove duplicates
            emailAddresses = emailAddresses.Distinct().ToList();

            // Send the eamil
            await _emailServiceApiApi.SendEmailAsync(emailAddresses, emailFormViewModel.Subject, emailFormViewModel.Message);

            return true;
        }
    }
}