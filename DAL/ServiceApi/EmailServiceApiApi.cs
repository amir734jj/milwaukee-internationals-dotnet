using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using DAL.Extensions;
using DAL.Interfaces;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using NETCore.MailKit.Core;

namespace DAL.ServiceApi
{
    public class EmailServiceApi : IEmailServiceApi
    {
        private readonly IEmailService _emailServiceApi;

        private readonly bool _connected;
        
        public EmailServiceApi()
        {
            _connected = false;
        }
        
        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="emailServiceApi"></param>
        public EmailServiceApi(IEmailService emailServiceApi)
        {
            _connected = true;
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
            return _connected ? _emailServiceApi.SendAsync(emailAddress, emailSubject, emailText, true) : Task.CompletedTask;
        }
        
        /// <summary>
        /// Send the email
        /// </summary>
        /// <param name="emailAddresses"></param>
        /// <param name="emailSubject"></param>
        /// <param name="emailText"></param>
        /// <returns></returns>
        public async Task SendEmailAsync(IEnumerable<string> emailAddresses, string emailSubject, string emailText)
        {
            // Delay task
            var delayTask = Task.Delay(10);
                
            // Start of the chain
            var chainTask = Task.Run(() => { });
                
            // Chain the tasks
            emailAddresses.ForEach(x =>
            {
                // ReSharper disable once AccessToModifiedClosure
                chainTask = chainTask
                    .ContinueWith(_ => SendEmailAsync(x, emailSubject, emailText))
                    .ContinueWith(_ => delayTask);
            });
                
            // Start the chain
            chainTask = chainTask.ContinueWith(_ => Task.CompletedTask);

            // Await
            await chainTask;
        }
    }
}