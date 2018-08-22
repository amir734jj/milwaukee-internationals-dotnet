using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using DAL.Extensions;
using DAL.Interfaces;
using Mailjet.Client;
using Mailjet.Client.Resources;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Newtonsoft.Json.Linq;
using NETCore.MailKit.Core;

namespace DAL.ServiceApi
{
    public class EmailServiceApi : IEmailServiceApi
    {
        private readonly IEmailService _emailServiceApi;

        private readonly bool _connected;
        private readonly IMailjetClient _mailjetClient;

        public EmailServiceApi()
        {
            _connected = false;
        }

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="emailServiceApi"></param>
        /// <param name="mailjetClient"></param>
        public EmailServiceApi(IEmailService emailServiceApi, IMailjetClient mailjetClient)
        {
            _connected = true;
            _emailServiceApi = emailServiceApi;
            _mailjetClient = mailjetClient;
        }

        /// <summary>
        /// Send the email
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="emailSubject"></param>
        /// <param name="emailHtml"></param>
        /// <returns></returns>
        public async Task SendEmailAsync(string emailAddress, string emailSubject, string emailHtml)
        {
            // return _connected ? _emailServiceApi.SendAsync(emailAddress, emailSubject, emailText, true) : Task.CompletedTask;
            if (_connected)
            {
                var request = new MailjetRequest { Resource = Send.Resource }
                    .Property(Send.FromEmail, "tourofmilwaukee@gmail.com")
                    .Property(Send.FromName, "Milwaukee-Internationals")
                    .Property(Send.Subject, emailSubject)
                    .Property(Send.HtmlPart, emailHtml)
                    .Property(Send.Recipients, new JArray
                    {
                        new JObject
                        {
                            {"Email", emailAddress}
                        }
                    });
                
                var response = await _mailjetClient.PostAsync(request);
            }
        }

        /// <summary>
        /// Send the email
        /// </summary>
        /// <param name="emailAddresses"></param>
        /// <param name="emailSubject"></param>
        /// <param name="emailHtml"></param>
        /// <returns></returns>
        public async Task SendEmailAsync(IEnumerable<string> emailAddresses, string emailSubject, string emailHtml)
        {
            var tasks = emailAddresses.Select(x => Task.Delay(TimeSpan.FromSeconds(1)).ContinueWith(_ => SendEmailAsync(x, emailSubject, emailHtml))).ToArray();

            await Task.WhenAll(tasks);
        }
    }
}