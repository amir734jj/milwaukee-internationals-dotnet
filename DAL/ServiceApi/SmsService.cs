using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Interfaces;
using Microsoft.Extensions.Logging;
using Models.Constants;
using Telnyx;

namespace DAL.ServiceApi;

public class SmsService : ISmsService
{
    private readonly ILogger<SmsService> _logger;
    private readonly string _senderPhoneNumber;
    private readonly GlobalConfigs _globalConfigs;

    public SmsService(string telnyxApiKey, string senderPhoneNumber,  GlobalConfigs globalConfigs, ILogger<SmsService> logger)
    {
        _senderPhoneNumber = senderPhoneNumber;
        _globalConfigs = globalConfigs;
        _logger = logger;
        TelnyxConfiguration.SetApiKey(telnyxApiKey);
    }

    public async Task SendMessage(string phoneNumber, string message)
    {
        if (!string.IsNullOrWhiteSpace(phoneNumber))
        { 
            // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
            _logger.LogInformation("Sending SMS to {}", phoneNumber);

            var service = new MessagingSenderIdService();
            var options = new NewMessagingSenderId
            {
                From = SimplifyPhoneNumber(_globalConfigs.SMSTestMode ? ApiConstants.SitePhoneNumber : phoneNumber),
                To = SimplifyPhoneNumber(phoneNumber),
                Text = message
            };
            
            var messageResponse = await service.CreateAsync(options);
            
            _logger.LogInformation("SMS sent successfully {}", messageResponse);
        }

        return;

        // This is needed because of this library
        static string SimplifyPhoneNumber(string x) => x.Replace("-", "").Replace(" ", "");
    }

    public async Task SendMessage(IEnumerable<string> phoneNumbers, string message)
    {
        await Task.WhenAll(phoneNumbers.Select(phoneNumber => SendMessage(phoneNumber, message)));
    }
}