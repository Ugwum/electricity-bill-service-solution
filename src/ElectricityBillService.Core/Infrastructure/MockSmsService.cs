using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricityBillService.Infrastructure.SmsNotification
{
    public class MockSmsService : ISmsService
    {
        public async Task SendSmsAsync(string phoneNumber, string message)
        {
            // Simulate sending an SMS
            Console.WriteLine($"Mock SMS sent to {phoneNumber}: {message}");
            await Task.CompletedTask; // Simulate async work
        }
    }
}
