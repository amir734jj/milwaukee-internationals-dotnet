using System;
using System.Threading.Tasks;
using DAL.Interfaces;
using NETCore.MailKit.Core;

namespace DAL.ServiceApi
{
    public class EmailServiceApi : IEmailServiceApi
    {
        private readonly IEmailService _emailServiceApi;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="emailServiceApi"></param>
        public EmailServiceApi(IEmailService emailServiceApi)
        {
            _emailServiceApi = emailServiceApi;
        }

        /// <summary>
        /// Send the email
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="emailSubject"></param>
        /// <param name="emailText"></param>
        /// <returns></returns>
        public Task SendEmailAsync(string emailAddress, string emailSubject, string emailText)
        {
            _emailServiceApi.SendAsync(emailAddress, emailSubject, emailText);
            
            return Task.CompletedTask;
        }
    }
}