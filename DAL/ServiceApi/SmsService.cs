using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Interfaces;
using Microsoft.Extensions.Logging;
using Models.Constants;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace DAL.ServiceApi;

public class SmsService : ISmsService
{
    private readonly ILogger<SmsService> _logger;
    private readonly string _senderPhoneNumber;
    private readonly GlobalConfigs _globalConfigs;

    public SmsService(string sid, string token, string senderPhoneNumber,  GlobalConfigs globalConfigs, ILogger<SmsService> logger)
    {
        _logger = logger;
        _senderPhoneNumber = senderPhoneNumber;
        _globalConfigs = globalConfigs;

        TwilioClient.Init(sid, token);
    }

    public async Task SendMessage(string phoneNumber, string message)
    {
        if (!string.IsNullOrWhiteSpace(phoneNumber))
        { 
            // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
            _logger.LogInformation("Sending SMS to {}", phoneNumber);

            var messageOptions = new CreateMessageOptions(new PhoneNumber(_globalConfigs.SMSTestMode ? ApiConstants.SitePhoneNumber : phoneNumber))
            {
                From = new PhoneNumber(_senderPhoneNumber),
                Body = message,
                SendAsMms = false
            };

            var response = await MessageResource.CreateAsync(messageOptions);
            
            // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
            _logger.LogInformation("SMS sent successfully {}", response?.Body);
        }
    }

    public async Task SendMessage(IEnumerable<string> phoneNumbers, string message)
    {
        await Task.WhenAll(phoneNumbers.Select(phoneNumber => SendMessage(phoneNumber, message)));
    }
}