using Amazon.Runtime.Internal.Util;

namespace ElectricityBillService.Infrastructure
{
    public interface IEventPublisher
    {
        Task PublishAsync<T>(T eventMessage) where T : class;
    }

}
