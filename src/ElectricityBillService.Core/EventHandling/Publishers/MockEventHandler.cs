
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricityBillService.Infrastructure.EventPublishers
{
    public class MockEventHandler : IEventPublisher
    {
        public Task PublishAsync<T>(T eventMessage) where T : class
        {
            Console.WriteLine($"Event Published: {eventMessage.GetType().Name}");
            return Task.CompletedTask;  
        }
    }
}
