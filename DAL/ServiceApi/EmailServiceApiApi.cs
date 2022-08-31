using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Interfaces;
using Mailjet.Client;
using Mailjet.Client.TransactionalEmails;
using Microsoft.Extensions.Logging;
using Models.Constants;
using NETCore.MailKit.Core;

namespace DAL.ServiceApi
{
    public class EmailServiceApi : IEmailServiceApi
    {
        private readonly bool _connected;

        private readonly IEmailService _emailServiceApi;
        private readonly IMailjetClient _mailJetClient;
        private readonly GlobalConfigs _globalConfigs;
        private readonly ILogger<EmailServiceApi> _logger;

        public EmailServiceApi()
        {
            _connected = false;
        }

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="emailServiceApi"></param>
        /// <param name="mailJetClient"></param>
        /// <param name="globalConfigs"></param>
        /// <param name="logger"></param>
        public EmailServiceApi(IEmailService emailServiceApi, IMailjetClient mailJetClient, GlobalConfigs globalConfigs, ILogger<EmailServiceApi> logger)
        {
            _connected = true;
            _emailServiceApi = emailServiceApi;
            _mailJetClient = mailJetClient;
            _globalConfigs = globalConfigs;
            _logger = logger;
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
            // Original GMail service
            // return _connected ? _emailServiceApi.SendAsync(emailAddress, emailSubject, emailText, true) : Task.CompletedTask;

            if (_connected && !string.IsNullOrWhiteSpace(emailAddress))
            {
                var task = Task.Delay(TimeSpan.FromSeconds(1)).ContinueWith(async _ =>
                {
                    // construct your email with builder
                    var email = new TransactionalEmailBuilder()
                        .WithFrom(new SendContact(ApiConstants.SiteEmail))
                        .WithSubject(emailSubject)
                        .WithHtmlPart(emailHtml)
                        .WithCc(new SendContact(ApiConstants.SiteEmail))
                        .WithTo(new SendContact(_globalConfigs.EmailTestMode ? ApiConstants.SiteEmail : emailAddress))
                        .Build();

                    // invoke API to send email
                    var response = await _mailJetClient.SendTransactionalEmailAsync(email);
                    
                    // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
                    _logger.LogInformation("Email sent successfully {}", response?.ToString());
                });

                await task;
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
            var tasks = emailAddresses.Select(x => SendEmailAsync(x, emailSubject, emailHtml)).ToArray();

            await Task.WhenAll(tasks);
        }
    }
}