using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace ElectricityBillService.Infrastructure.SmsNotification
{
    public class TwilioSmsService : ISmsService
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _fromPhoneNumber;
        private readonly ILogger<TwilioSmsService> _logger;

        public TwilioSmsService(string accountSid, string authToken, string fromPhoneNumber)
        {
            _accountSid = accountSid;
            _authToken = authToken;
            _fromPhoneNumber = fromPhoneNumber;

            TwilioClient.Init(_accountSid, _authToken);
        }

        public async Task SendSmsAsync(string phoneNumber, string message)
        {
            try
            {
                var messageOptions = new CreateMessageOptions(new PhoneNumber(phoneNumber))
                {
                    From = new PhoneNumber(_fromPhoneNumber),
                    Body = message
                };

                var msg = await MessageResource.CreateAsync(messageOptions);
                Console.WriteLine($"SMS sent to {phoneNumber}: {msg.Sid}");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"An error occurrend sending sms for phone {phoneNumber}");
            }
           
        }
    }
}
